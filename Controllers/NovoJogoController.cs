using BitBeakAPI.Models;
using BitBeakAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NovoJogoController : ControllerBase
    {
        private readonly BitBeakContext _context;
        private readonly QuestaoService _questaoService;

        public NovoJogoController(BitBeakContext objContext, QuestaoService objQuestaoService)
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
            // Verificar a resposta do usuário
            var objRespostaRequest = new VerificarRespostaRequest
            {
                IdQuestao = objRequest.IdQuestaoAleatoria,
                IdOpcao = objRequest.IdOpcaoEscolhidaUsuario,
                IdUsuario = objRequest.IdUsuario
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

            // NECESSÁRIO AJUSTAR
            if (objRequest.ContadorAcertos == 5)
            {
                // Atribuir recompensas - XP, Penas e Nível concluído
                return Ok("Parabéns! Você ganhou 50 XP e 5 penas.");
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
                return Ok(new { Questao = objDadosQuestao, 
                                IdUsuario = objRequest.IdUsuario, 
                                QuestoesRespondidas = objRequest.QuestoesRespondidas, 
                                ContadorAcertos = objRequest.ContadorAcertos, 
                                ContadorErros = objRequest.ContadorErros });
            }
            else
            {
                return NotFound("Nenhuma questão encontrada.");
            }
        }

        /// <summary>
        /// Função para verificar se a resposta foi correta
        /// </summary>
        /// <param name="objRequest"></param>
        /// <returns></returns>
        [HttpPost("VerificarResposta")]
        public async Task<ActionResult<VerificarRespostaResponse>> VerificarResposta(VerificarRespostaRequest objRequest)
        {
            var objQuestao = await _context.Questoes
                .Include(q => q.Opcoes)
                .Include(q => q.Nivel)
                .Include(q => q.Lacunas)
                .FirstOrDefaultAsync(q => q.IdQuestao == objRequest.IdQuestao);

            if (objQuestao == null)
            {
                return NotFound("Questão não encontrada.");
            }

            var objOpcaoEscolhida = objQuestao.Opcoes.FirstOrDefault(o => o.IdOpcao == objRequest.IdOpcao);

            if (objOpcaoEscolhida == null)
            {
                return NotFound("Opção de resposta não encontrada.");
            }

            var objUsuario = await _context.Usuarios.FindAsync(objRequest.IdUsuario);

            if (objUsuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (objQuestao.Nivel == null)
            {
                return BadRequest("A questão não está associada a um nível.");
            }

            var objTrilhaProgresso = await _context.UsuarioTrilhaProgresso
                .FirstOrDefaultAsync(p => p.IdUsuario == objRequest.IdUsuario && p.IdTrilha == objQuestao.Nivel.IdTrilha);

            // AJUSTAR TRILHA DE PROGRESSO
            if (objTrilhaProgresso == null)
            {
                objTrilhaProgresso = new ModelUsuarioTrilhaProgresso
                {
                    IdUsuario = objRequest.IdUsuario,
                    NivelUsuario = objQuestao.Nivel.Nivel,
                    ExperienciaUsuario = 0,
                    Penas = 0,
                    Erros = 0
                };
                _context.UsuarioTrilhaProgresso.Add(objTrilhaProgresso);
                await _context.SaveChangesAsync();
            }

            bool blnAcertou = objOpcaoEscolhida.Correta;

            // AJUSTAR ACERTO E ERRO
            if (blnAcertou)
            {
                objTrilhaProgresso.ExperienciaUsuario++;
            }
            else
            {
                objTrilhaProgresso.Erros++;
            }

            // AdicionaR a questão respondida à tabela de questões respondidas
            var objQuestaoRespondida = new ModelQuestaoRespondida
            {
                IdUsuario = objRequest.IdUsuario,
                IdQuestao = objRequest.IdQuestao
            };
            _context.QuestoesRespondidas.Add(objQuestaoRespondida);

            if (objTrilhaProgresso.ExperienciaUsuario >= 5)
            {
                // Concluir o nível
                objTrilhaProgresso.NivelUsuario++;
                objTrilhaProgresso.ExperienciaUsuario = 0; // Resetar a experiência para o próximo nível
                objUsuario.Penas += 10;

                // Atualizar a experiência baseada nos erros
                int intExpGanha = objTrilhaProgresso.Erros switch
                {
                    0 => 50,
                    1 => 45,
                    2 => 40,
                    3 => 30,
                    4 => 20,
                    _ => 10
                };

                objUsuario.ExperienciaUsuario += intExpGanha;
                objTrilhaProgresso.Erros = 0; // Resetar os erros após a conclusão do nível

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
            }

            await _context.SaveChangesAsync();

            return Ok(new VerificarRespostaResponse
            {
                Acertou = blnAcertou,
                NivelAtual = objTrilhaProgresso.NivelUsuario,
                ExperienciaAtual = objTrilhaProgresso.ExperienciaUsuario,
                PenasAtuais = objUsuario.Penas
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
            public HashSet<int>? QuestoesRespondidas { get; set; }
            public int ContadorAcertos { get; set; }
            public int ContadorErros { get; set; }
        }

        public class VerificarRespostaRequest
        {
            public int IdUsuario { get; set; }
            public int IdQuestao { get; set; }
            public int IdOpcao { get; set; }
        }

        public class VerificarRespostaResponse
        {
            public bool Acertou { get; set; }
            public int NivelAtual { get; set; }
            public int ExperienciaAtual { get; set; }
            public int PenasAtuais { get; set; }
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
