using BitBeakAPI.Models;
using BitBeakAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JogoController : ControllerBase
    {
        private readonly BitBeakContext _context;
        private readonly QuestaoService _questaoService;

        public JogoController(BitBeakContext objContext, QuestaoService objQuestaoService)
        {
            _context = objContext;
            _questaoService = objQuestaoService;
        }

        /// <summary>
        /// Função que INICIA um nível 
        /// </summary>
        /// <param name="objIniciarNivel">Obrigatório enviar o IdUsuario, IdTrilha e IdNivelTrilha</param>
        /// <returns></returns>
        [HttpPost("IniciarNivel")]
        public async Task<ActionResult> IniciarNivel([FromBody] IniciarNivelRequest objIniciarNivel)
        {
            var objQuestoesRespondidas = new HashSet<int>();
            int intContadorAcertos = 0;
            int intContadorErros = 0;

            var objPrimeiraQuestao = await ObterProximaQuestao(objIniciarNivel.IdTrilha, 
                                                               objIniciarNivel.IdNivelTrilha, 
                                                               objQuestoesRespondidas);

            if (objPrimeiraQuestao is ModelQuestao objDadosQuestao)
            {
                // Retornar a primeira questão para o frontend
                return Ok(new { Questao = objDadosQuestao, 
                                IdUsuario = objIniciarNivel.IdUsuario, 
                                QuestoesRespondidas = objQuestoesRespondidas, 
                                ContadorAcertos = intContadorAcertos, 
                                ContadorErros = intContadorErros });
            }
            else
            {
                return NotFound("Nenhuma questão encontrada para iniciar o nível.");
            }
        }

        /// <summary>
        /// Função para buscar uma questão aleatória
        /// </summary>
        /// <param name="idTrilha"></param>
        /// <param name="idNivelTrilha"></param>
        /// <param name="objQuestoesRespondidas"></param>
        /// <returns></returns>
        private async Task<ModelQuestao?> ObterProximaQuestao(int idTrilha, int idNivelTrilha, HashSet<int> objQuestoesRespondidas)
        {
            // Obter o resultado da busca por uma questão aleatória
            var objResultadoAleatoria = await ObterIdQuestaoAleatoria(idTrilha, idNivelTrilha);

            // Se bem-sucedido - Extrair o ID da questão aleatória do resultado
            if (objResultadoAleatoria.Result is OkObjectResult okResult && okResult.Value is int idQuestaoAleatoria)
            {
                // Verificar se a questão já foi respondida (Se sim, busca outra)
                if (objQuestoesRespondidas.Contains(idQuestaoAleatoria))
                {
                    return await ObterProximaQuestao(idTrilha, idNivelTrilha, objQuestoesRespondidas); // Tenta obter outra questão
                }

                // Buscar os dados da questão usando o ID obtido
                var objResultadoDados = await _questaoService.ListarDadosQuestao(idQuestaoAleatoria);

                if (objResultadoDados is ModelQuestao objDadosQuestao)
                {
                    return objDadosQuestao;
                }
                else
                {
                    return null; // Nenhuma questão encontrada para o ID
                }
            }
            else
            {
                return null; // Falha ao obter uma questão aleatória ou o resultado não é do tipo esperado
            }
        }

        /// <summary>
        /// Função para enviar a resposta do usuário
        /// </summary>
        /// <param name="objRequest"></param>
        /// <returns></returns>
        [HttpPost("ResponderQuestao")]
        public async Task<ActionResult> ResponderQuestao(ResponderQuestaoRequest objRequest)
        {
            // Carregar o usuário existente do banco de dados
            ModelUsuario? objUsuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == objRequest.IdUsuario);

            try
            {
                if (objUsuario == null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                // Verificar a resposta do usuário
                var objRespostaRequest = new VerificarRespostaRequest
                {
                    IdQuestao = objRequest.IdQuestaoAleatoria,
                    IdOpcao = objRequest.IdOpcaoEscolhidaUsuario,
                    IdUsuario = objRequest.IdUsuario,
                    RespostaUsuario = objRequest.RespostaUsuario,
                    RespostasLacunas = objRequest.RespostasLacunas.Select(objLacuna => new VerificarRespostaRequest.RespostaLacuna
                                                                            {IdLacuna = objLacuna.IdLacuna, RespostaColunaA = objLacuna.RespostaColunaA, RespostaColunaB = objLacuna.RespostaColunaB }
                                                                         ).ToList(),
        };

                var objResultadoResposta = await VerificarResposta(objRespostaRequest);

                if (objResultadoResposta.Result is OkObjectResult okResultResposta &&
                   ((VerificarRespostaResponse)okResultResposta.Value!).Acertou)
                {
                    objRequest.ContadorAcertos++;                                        // Adicionar +1 no contador de acertos
                    objRequest.QuestoesRespondidas!.Add(objRequest.IdQuestaoAleatoria);  // Inserir id da questão em Questoes Respondidas
                }
                else
                {
                    objRequest.ContadorErros++; // Adicionar +1 no contador de erros
                }

                if (objRequest.ContadorAcertos == 5)
                {
                    int intXpGanho = 0;
                    int intPenasGanhas = 0;

                    if (objRequest.ContadorErros == 2)
                    {
                        intXpGanho = 35;
                        intPenasGanhas = 5;
                    }
                    else if (objRequest.ContadorErros == 1)
                    {
                        intXpGanho = 40;
                        intPenasGanhas = 8;
                    }
                    else if (objRequest.ContadorErros == 0)
                    {
                        intXpGanho = 50;
                        intPenasGanhas = 10;
                    }

                    objUsuario.ExperienciaUsuario += intXpGanho;
                    objUsuario.Penas += intPenasGanhas;

                    // Verificar se o usuário subiu de nível
                    var objNivelUsuarioAtual = await _context.NiveisUsuario
                        .FirstOrDefaultAsync(n => n.NivelUsuario == objUsuario.NivelUsuario);

                    if (objNivelUsuarioAtual == null)
                    {
                        return NotFound("Nível do usuário não encontrado.");
                    }

                    if (objUsuario.ExperienciaUsuario >= objNivelUsuarioAtual.ExperienciaNecessaria)
                    {
                        // Subir de nível
                        objUsuario.NivelUsuario++;
                        objUsuario.ExperienciaUsuario -= objNivelUsuarioAtual.ExperienciaNecessaria;
                    }

                    _context.Usuarios.Update(objUsuario);
                    await _context.SaveChangesAsync();

                    return Ok($"Parabéns! Você conclui o nível {objRequest.IdNivelTrilha}!");
                }
            
                else if (objRequest.ContadorErros == 3)
                {

                    return Ok("Jogo finalizado. Tente novamente!");
                }

                var objProximaQuestao = await ObterProximaQuestao(objRequest.IdTrilha,
                                                                  objRequest.IdNivelTrilha,
                                                                  objRequest.QuestoesRespondidas!);

                if (objProximaQuestao is ModelQuestao objDadosQuestao)
                {
                    // Retornar a próxima questão para o frontend
                    return Ok(new
                    {
                        Questao = objDadosQuestao,
                        IdUsuario = objRequest.IdUsuario,
                        QuestoesRespondidas = objRequest.QuestoesRespondidas,
                        ContadorAcertos = objRequest.ContadorAcertos,
                        ContadorErros = objRequest.ContadorErros
                    });
                }
                else
                {
                    return NotFound("Nenhuma questão encontrada.");
                }
            }
            catch
            {
                return BadRequest("Erro na aplicação!");
            }
        }

        /// <summary>
        /// Função para verificar se a resposta foi correta
        /// </summary>
        /// <param name="objRequest"></param>
        /// <returns></returns>
        public async Task<ActionResult<VerificarRespostaResponse>> VerificarResposta(VerificarRespostaRequest objRequest)
        {
            var objQuestao = await _context.Questoes
                .Include(q => q.Opcoes)
                .Include(q => q.Lacunas)
                .Include(q => q.CodeFill)
                .Include(q => q.Nivel)
                .FirstOrDefaultAsync(q => q.IdQuestao == objRequest.IdQuestao);

            if (objQuestao == null)
            {
                return NotFound($"Questão com ID {objRequest.IdQuestao} não encontrada.");
            }

            if (objQuestao.Nivel == null)
            {
                return NotFound("Informações do nível para a questão não estão disponíveis.");
            }

            bool blnAcertou = false; 

            switch (objQuestao.Tipo)
            {
                case TipoQuestao.Pergunta:
                    blnAcertou = objQuestao.Opcoes.Any(o => o.IdOpcao == objRequest.IdOpcao && o.Correta);
                    break;
                case TipoQuestao.Lacuna:
                    if (objQuestao.Lacunas != null && objRequest.RespostasLacunas != null)
                    {
                        blnAcertou = objRequest.RespostasLacunas.All(respostaLacuna =>
                                                objQuestao.Lacunas.Any(lacuna =>
                                                                       lacuna.IdLacuna == respostaLacuna.IdLacuna &&
                                                                       lacuna.ColunaA == respostaLacuna.RespostaColunaA &&
                                                                       lacuna.ColunaB == respostaLacuna.RespostaColunaB));
                    }
                    else
                    {
                        blnAcertou = false; // Assume false se não houver lacunas ou respostas de lacuna
                    }
                    break;
                case TipoQuestao.CodeFill:

                    blnAcertou = objQuestao.CodeFill.All(cf =>
                    {
                        string strRespostaEsperadaFormatada = cf.RespostaEsperada!.Replace(" ", "").ToUpper();
                        string strRespostaUsuarioFormatada = objRequest.RespostaUsuario!.Replace(" ", "").ToUpper();

                        Console.WriteLine($"Resposta Esperada (formatada): {strRespostaEsperadaFormatada}");
                        Console.WriteLine($"Resposta do Usuário (formatada): {strRespostaUsuarioFormatada}");

                        return strRespostaEsperadaFormatada == strRespostaUsuarioFormatada;
                    });
                    break;
            }

            return Ok(new VerificarRespostaResponse
            {
                Acertou = blnAcertou,
                NivelAtual = objQuestao.Nivel!.Nivel
            });
        }

        [HttpGet("{intIdTrilha}/Niveis/{intIdNivel}/ObterIdQuestaoAleatoria")]
        public async Task<ActionResult<int>> ObterIdQuestaoAleatoria(int intIdTrilha, int intIdNivel)
        {
            var objNivelTrilha = await _context.Trilhas
                                        .Where(t => t.IdTrilha == intIdTrilha)
                                        .SelectMany(t => t.Niveis)
                                        .Include(n => n.Questoes) // Garantindo que as questões sejam carregadas
                                        .FirstOrDefaultAsync(n => n.Nivel == intIdNivel);

            if (objNivelTrilha == null || !objNivelTrilha.Questoes.Any())
            {
                return NotFound("Nível não encontrado ou sem questões.");
            }

            var objQuestaoAleatoria = objNivelTrilha.Questoes
                .OrderBy(q => Guid.NewGuid())
                .Select(q => q.IdQuestao)
                .FirstOrDefault();

            if (objQuestaoAleatoria != default)
            {
                return Ok(objQuestaoAleatoria);
            }
            else
            {
                return NotFound("Não foi possível encontrar uma questão aleatória.");
            }
        }

        [HttpPost("{intIdUsuario}/AdicionarExperiencia")]
        public async Task<IActionResult> AdicionarExperiencia(int intIdUsuario, [FromBody] AdicionarExperienciaRequest objRequest)
        {
            var objUsuario = await _context.Usuarios.FindAsync(intIdUsuario);

            if (objUsuario == null)
            {
                return NotFound();
            }

            // Adicionar a experiência ao usuário
            objUsuario.ExperienciaUsuario += objRequest.Experiencia;

            // Verificar se o usuário pode upar de nível
            while (true)
            {
                var objNivelUsuario = await _context.NiveisUsuario
                    .FirstOrDefaultAsync(nu => nu.NivelUsuario == objUsuario.NivelUsuario);

                if (objNivelUsuario == null || objUsuario.ExperienciaUsuario < objNivelUsuario.ExperienciaNecessaria)
                {
                    break;
                }

                // Upar de nível
                objUsuario.NivelUsuario++;
                objUsuario.ExperienciaUsuario -= objNivelUsuario.ExperienciaNecessaria; // Reduzir a experiência acumulada
            }

            await _context.SaveChangesAsync();

            // Retornar a experiência e nível atualizados do usuário
            var objResponse = new UsuarioExperienciaResponse
            {
                NivelUsuario = objUsuario.NivelUsuario,
                ExperienciaUsuario = objUsuario.ExperienciaUsuario
            };

            return Ok(objResponse);
        }

        #region Classes Necessárias

        public class IniciarNivelRequest
        {
            public int IdTrilha { get; set; }
            public int IdNivelTrilha { get; set; }
            public int IdUsuario { get; set; }
        }

        public class ResponderQuestaoRequest
        {
            public int IdTrilha { get; set; }
            public int IdNivelTrilha { get; set; }
            public int IdUsuario { get; set; }
            public int IdQuestaoAleatoria { get; set; }
            public int IdOpcaoEscolhidaUsuario { get; set; }
            public List<RespostaLacuna> RespostasLacunas { get; set; } = new List<RespostaLacuna>();

            public string RespostaUsuario { get; set; } = string.Empty;

            public HashSet<int>? QuestoesRespondidas { get; set; }
            public int ContadorAcertos { get; set; }
            public int ContadorErros { get; set; }
        }

        public class VerificarRespostaRequest
        {
            public int IdUsuario { get; set; }
            public int IdQuestao { get; set; }
            public int IdOpcao { get; set; } // Usado para questões do tipo opção

            public string RespostaUsuario { get; set; } = string.Empty;
            public List<RespostaLacuna> RespostasLacunas { get; set; } = new List<RespostaLacuna>(); // Usado para questões do tipo lacuna
            
            // Classe interna para respostas de lacuna
            public class RespostaLacuna
            {
                public int IdLacuna { get; set; }
                public string? RespostaColunaA { get; set; }
                public string? RespostaColunaB { get; set; }
            }
        }

        public class VerificarRespostaResponse
        {
            public bool Acertou { get; set; }
            public int NivelAtual { get; set; }
            public int ExperienciaAtual { get; set; }
            public int PenasAtuais { get; set; }
        }

        public class RespostaLacuna
        {
            public int IdLacuna { get; set; }
            public string? RespostaColunaA { get; set; }
            public string? RespostaColunaB { get; set; }
        }

        public class AdicionarExperienciaRequest
        {
            public int Experiencia { get; set; }
        }

        public class UsuarioExperienciaResponse
        {
            public int NivelUsuario { get; set; }
            public int ExperienciaUsuario { get; set; }
        }

        #endregion
    }
}
