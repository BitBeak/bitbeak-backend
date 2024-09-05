using BitBeakAPI.Models;
using BitBeakAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DotNetEnv;
using System.Net;
using System.Security.Authentication;
using System.Text.Json.Serialization;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JogoController : ControllerBase
    {
        private readonly BitBeakContext _context;
        private readonly QuestaoService _questaoService;
        private readonly string _strApiKey; 


        public JogoController(BitBeakContext objContext, QuestaoService objQuestaoService)
        {
            _context = objContext;
            _questaoService = objQuestaoService;

            Env.Load();  // Carrega as variáveis de ambiente do arquivo .env
            _strApiKey = Environment.GetEnvironmentVariable("RAPIDAPI_KEY")!;
        }

        [NonAction]
        public async Task<bool> ExecutarCodigoRemotamente(string strCodigoUsuario, string strEntradaEsperada, string strSaidaEsperada)
        {
            try
            {
                HttpClient HttpClient = new();

                // Codificar em Base64
                string strBase64SourceCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(strCodigoUsuario));
                string strBase64Stdin = Convert.ToBase64String(Encoding.UTF8.GetBytes(strEntradaEsperada));
                string strBase64ExpectedOutput = Convert.ToBase64String(Encoding.UTF8.GetBytes(strSaidaEsperada));

                var objPayload = new
                {
                    source_code = strBase64SourceCode,
                    language_id = 63,
                    stdin = strBase64Stdin,
                    expected_output = strBase64ExpectedOutput
                };

                string strJson = JsonSerializer.Serialize(objPayload);
                StringContent objContent = new StringContent(strJson, Encoding.UTF8, "application/json");

                HttpRequestMessage objRequest = new()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://judge0-ce.p.rapidapi.com/submissions?base64_encoded=true&wait=true"),
                    Content = objContent,
                    Headers =
                            {
                                { "x-rapidapi-key", _strApiKey },
                                { "x-rapidapi-host", "judge0-ce.p.rapidapi.com" },
                            },
                };

                using (var response = await HttpClient.SendAsync(objRequest))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string strBody = await response.Content.ReadAsStringAsync();
                        Judge0Response objResult = JsonSerializer.Deserialize<Judge0Response>(strBody)!;

                        try
                        {
                            if (!string.IsNullOrEmpty(objResult.Stdout))
                            {
                                string strDecodedStdout = Encoding.UTF8.GetString(Convert.FromBase64String(objResult.Stdout.Trim()));
                                Console.WriteLine("Decoded Stdout:");
                                Console.WriteLine(strDecodedStdout);
                                
                                string strDecodedExpectedOutput = Encoding.UTF8.GetString(Convert.FromBase64String(strBase64ExpectedOutput.Trim()));
                                Console.WriteLine("Decoded Expected:");
                                Console.WriteLine(strDecodedExpectedOutput);

                                return strDecodedStdout.Trim().ToUpper() == strDecodedExpectedOutput.Trim().ToUpper();
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else
                    {
                        string strErrorMessage = await response.Content.ReadAsStringAsync();
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #region Iniciar Nível
        /// <summary>
        /// Função que INICIA um nível 
        /// </summary>
        /// <param name="objIniciarNivel">Obrigatório enviar o IdUsuario, IdTrilha e IdNivelTrilha</param>
        /// <returns></returns>
        [HttpPost("IniciarNivel")]
        public async Task<ActionResult> IniciarNivel([FromBody] IniciarNivelRequest objIniciarNivel)
        {
            try
            {
                var objQuestoesRespondidas = new HashSet<int>();
                var tiposQuestoesRespondidas = new Dictionary<TipoQuestao, int>(); 
                int intContadorAcertos = 0;
                int intContadorErros = 0;

                var objPrimeiraQuestao = await ObterProximaQuestao(objIniciarNivel.IdTrilha,
                                                                   objIniciarNivel.IdNivelTrilha,
                                                                   objQuestoesRespondidas,
                                                                   tiposQuestoesRespondidas); 

                if (objPrimeiraQuestao is ModelQuestao objDadosQuestao)
                {
                    return Ok(new
                    {
                        Questao = objDadosQuestao,
                        IdUsuario = objIniciarNivel.IdUsuario,
                        QuestoesRespondidas = objQuestoesRespondidas,
                        TiposQuestoesRespondidas = tiposQuestoesRespondidas,
                        ContadorAcertos = intContadorAcertos,
                        ContadorErros = intContadorErros
                    });
                }
                else
                {
                    return NotFound("Nenhuma questão encontrada para iniciar o nível.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Função para buscar uma questão aleatória
        /// </summary>
        /// <param name="idTrilha"></param>
        /// <param name="idNivelTrilha"></param>
        /// <param name="objQuestoesRespondidas"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<ModelQuestao?> ObterProximaQuestao(int idTrilha, int idNivelTrilha, HashSet<int> objQuestoesRespondidas, Dictionary<TipoQuestao, int> tiposQuestoesRespondidas)
        {
            var objResultadoAleatoria = await ObterIdQuestaoAleatoria(idTrilha, idNivelTrilha);

            if (objResultadoAleatoria.Result is OkObjectResult okResult && okResult.Value is int idQuestaoAleatoria)
            {
                if (objQuestoesRespondidas.Contains(idQuestaoAleatoria))
                {
                    return await ObterProximaQuestao(idTrilha, idNivelTrilha, objQuestoesRespondidas, tiposQuestoesRespondidas);
                }

                var objResultadoDados = await _questaoService.ListarDadosQuestao(idQuestaoAleatoria);

                if (objResultadoDados is ModelQuestao objDadosQuestao)
                {
                    if (tiposQuestoesRespondidas.ContainsKey(objDadosQuestao.Tipo) && tiposQuestoesRespondidas[objDadosQuestao.Tipo] >= 2)
                    {
                        return await ObterProximaQuestao(idTrilha, idNivelTrilha, objQuestoesRespondidas, tiposQuestoesRespondidas);
                    }

                    return objDadosQuestao;
                }
                else
                {
                    return null; 
                }
            }
            else
            {
                return null; 
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
                    IdNivel = objRequest.IdNivelTrilha,
                    IdTrilha = objRequest.IdTrilha,
                    RespostaUsuario = objRequest.RespostaUsuario,
                    RespostasLacunas = objRequest.RespostasLacunas.Select(objLacuna => new VerificarRespostaRequest.RespostaLacuna
                    { IdLacuna = objLacuna.IdLacuna, RespostaColunaA = objLacuna.RespostaColunaA, RespostaColunaB = objLacuna.RespostaColunaB }
                                                                         ).ToList(),
                };

                var objResultadoResposta = await VerificarResposta(objRespostaRequest);

                if (objResultadoResposta.Result is OkObjectResult okResultResposta &&
                    okResultResposta.Value is VerificarRespostaResponse objVerificarResposta)
                {
                    if (objVerificarResposta.Acertou)
                    {
                        objRequest.ContadorAcertos++;
                    }
                    else
                    {
                        objRequest.ContadorErros++;
                    }

                    objRequest.QuestoesRespondidas!.Add(objRequest.IdQuestaoAleatoria);

                    if (objRequest.TiposQuestoesRespondidas.ContainsKey(objVerificarResposta.TipoQuestao))
                    {
                        objRequest.TiposQuestoesRespondidas[objVerificarResposta.TipoQuestao]++;
                    }
                    else
                    {
                        objRequest.TiposQuestoesRespondidas[objVerificarResposta.TipoQuestao] = 1;
                    }
                }
                else
                {
                    return BadRequest("Erro ao verificar resposta.");
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
                    objUsuario.ExperienciaQuinzenalUsuario += intXpGanho;
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

                    // Chamar a função para registrar a conclusão do nível
                    var objResultadoConclusao = await ConcluirNivel(objUsuario.IdUsuario, objRequest.IdTrilha, objRequest.IdNivelTrilha);
                    if (objResultadoConclusao is BadRequestObjectResult)
                    {
                        return objResultadoConclusao; // Retornar qualquer erro ocorrido na conclusão
                    }

                    // Verificar se todos os níveis da trilha foram concluídos
                    bool blnTrilhaConcluida = await VerificarConclusaoTrilha(objUsuario.IdUsuario, objRequest.IdTrilha);
                    if (blnTrilhaConcluida)
                    {
                        var objResultadoConclusaoTrilha = await ConcluirTrilha(objUsuario.IdUsuario, objRequest.IdTrilha);
                        if (objResultadoConclusaoTrilha is BadRequestObjectResult)
                        {
                            return objResultadoConclusaoTrilha; // Retornar qualquer erro ocorrido na conclusão da trilha
                        }
                    }

                    return Ok($"Parabéns! Você concluiu o nível {objRequest.IdNivelTrilha}!");
                }

                else if (objRequest.ContadorErros == 3)
                {
                    return Ok("Jogo finalizado. Tente novamente!");
                }

                var objProximaQuestao = await ObterProximaQuestao(objRequest.IdTrilha,
                                                                  objRequest.IdNivelTrilha,
                                                                  objRequest.QuestoesRespondidas!,
                                                                  objRequest.TiposQuestoesRespondidas);

                if (objProximaQuestao is ModelQuestao objDadosQuestao)
                {
                    // Retornar a próxima questão para o frontend
                    return Ok(new
                    {
                        Questao = objDadosQuestao,
                        IdUsuario = objRequest.IdUsuario,
                        QuestoesRespondidas = objRequest.QuestoesRespondidas,
                        ContadorAcertos = objRequest.ContadorAcertos,
                        ContadorErros = objRequest.ContadorErros,
                        TiposQuestoesRespondidas = objRequest.TiposQuestoesRespondidas
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

        [NonAction]
        public async Task<ActionResult<VerificarRespostaResponse>> VerificarResposta(VerificarRespostaRequest objRequest)
        {
            var objQuestao = await _context.Questoes
                .Include(q => q.Opcoes)
                .Include(q => q.Lacunas)
                .Include(q => q.CodeFill)
                .Include(q => q.Nivel)
                .ThenInclude(n => n!.Trilha)
                .FirstOrDefaultAsync(q => q.IdQuestao == objRequest.IdQuestao);

            if (objQuestao == null)
            {
                return NotFound($"Questão com ID {objRequest.IdQuestao} não encontrada.");
            }

            if (objQuestao.Nivel == null)
            {
                return NotFound("Informações do nível para a questão não estão disponíveis.");
            }

            if (objQuestao.Nivel.IdNivel != objRequest.IdNivel)
            {
                return BadRequest("A questão não pertence ao nível especificado.");
            }

            if (objQuestao.Nivel.IdTrilha != objRequest.IdTrilha)
            {
                return BadRequest("O nível não pertence à trilha especificada.");
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
                        blnAcertou = objRequest.RespostasLacunas.All(objRespostaLacuna =>
                                                objQuestao.Lacunas.Any(objLacuna =>
                                                                       objLacuna.IdLacuna == objRespostaLacuna.IdLacuna &&
                                                                       objLacuna.ColunaA == objRespostaLacuna.RespostaColunaA &&
                                                                       objLacuna.ColunaB == objRespostaLacuna.RespostaColunaB));
                    }
                    else
                    {
                        blnAcertou = false; // Assume false se não houver lacunas ou respostas de lacuna
                    }
                    break;
                case TipoQuestao.CodeFill:

                    blnAcertou = objQuestao.CodeFill.All(objCodeFill =>
                    {
                        string strRespostaEsperadaFormatada = objCodeFill.RespostaEsperada!.Replace(" ", "").ToUpper();
                        string strRespostaUsuarioFormatada = objRequest.RespostaUsuario!.Replace(" ", "").ToUpper();

                        return strRespostaEsperadaFormatada == strRespostaUsuarioFormatada;
                    });
                    break;
                case TipoQuestao.Codificacao:
                    string strCodigoCompleto = $"function teste() {{\n {objRequest.RespostaUsuario} \n}} teste();";
                    blnAcertou = await ExecutarCodigoRemotamente(strCodigoCompleto, "", objQuestao.SolucaoEsperada);
                    break;
            }

            return Ok(new VerificarRespostaResponse
            {
                Acertou = blnAcertou,
                NivelAtual = objQuestao.Nivel!.Nivel,
                TipoQuestao = objQuestao.Tipo // Atribuindo o tipo da questão
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

            // Excluir questões do tipo Desafio
            var objQuestaoAleatoria = objNivelTrilha.Questoes
                .Where(q => q.Tipo != TipoQuestao.Desafio) 
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

        [NonAction]
        private async Task<ActionResult> ConcluirNivel(int idUsuario, int idTrilha, int idNivel)
        {
            try
            {
                // Verificar se o nível já foi concluído pelo usuário
                var nivelConcluido = await _context.UsuarioNiveisConcluidos
                    .FirstOrDefaultAsync(un => un.IdUsuario == idUsuario && un.IdTrilha == idTrilha && un.IdNivel == idNivel);
                if (nivelConcluido != null)
                {
                    return BadRequest("O usuário já concluiu este nível.");
                }

                // Criar o registro de conclusão do nível
                var novaNivelConclusao = new ModelUsuarioNivelConcluido
                {
                    IdUsuario = idUsuario,
                    IdTrilha = idTrilha,
                    IdNivel = idNivel
                };

                // Salvar no banco de dados
                _context.UsuarioNiveisConcluidos.Add(novaNivelConclusao);
                await _context.SaveChangesAsync();

                return Ok("Nível concluído com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        private async Task<bool> VerificarConclusaoTrilha(int idUsuario, int idTrilha)
        {
            // Buscar todos os níveis dessa trilha
            var niveisDaTrilha = await _context.NiveisTrilha.Where(n => n.IdTrilha == idTrilha).ToListAsync();

            // Verificar se todos os níveis da trilha foram concluídos pelo usuário
            foreach (var nivel in niveisDaTrilha)
            {
                var nivelConcluido = await _context.UsuarioNiveisConcluidos
                    .FirstOrDefaultAsync(un => un.IdUsuario == idUsuario && un.IdTrilha == idTrilha && un.IdNivel == nivel.IdNivel);
                if (nivelConcluido == null)
                {
                    return false; // Se algum nível não foi concluído, retorna falso
                }
            }

            return true; // Todos os níveis foram concluídos
        }

        [NonAction]
        private async Task<ActionResult> ConcluirTrilha(int idUsuario, int idTrilha)
        {
            try
            {
                // Verificar se a trilha já foi concluída pelo usuário
                var trilhaConcluida = await _context.UsuarioTrilhasConcluidas
                    .FirstOrDefaultAsync(ut => ut.IdUsuario == idUsuario && ut.IdTrilha == idTrilha);
                if (trilhaConcluida != null)
                {
                    return BadRequest("O usuário já concluiu esta trilha.");
                }

                // Criar o registro de conclusão da trilha
                var novaTrilhaConclusao = new ModelUsuarioTrilhaConcluida
                {
                    IdUsuario = idUsuario,
                    IdTrilha = idTrilha
                };

                // Salvar no banco de dados
                _context.UsuarioTrilhasConcluidas.Add(novaTrilhaConclusao);
                await _context.SaveChangesAsync();

                return Ok("Trilha concluída com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// Função para Ranking Quinzenal de jogadores com mais experiência ganha
        /// </summary>
        /// <param name="idUsuario">Obrigatório enviar o ID do Usuário</param>
        /// <returns></returns>
        [HttpGet("RankingQuinzenal/{intIdUsuario}")]
        public async Task<ActionResult> RankingQuinzenal(int intIdUsuario)
        {
            try
            {
                var objTopUsuarios = await _context.Usuarios
                                            .OrderByDescending(u => u.ExperienciaQuinzenalUsuario)
                                            .Take(10)
                                            .ToListAsync();

                var objRanking = objTopUsuarios.Select((objUsuario, intIndex) => new UsuarioRankingResponse
                {
                    IdUsuario = objUsuario.IdUsuario,
                    Nome = objUsuario.Nome,
                    ExperienciaQuinzenal = objUsuario.ExperienciaQuinzenalUsuario,
                    Posicao = intIndex + 1
                }).ToList();

                var objUsuarioAtual = await _context.Usuarios.FindAsync(intIdUsuario);
                if (objUsuarioAtual == null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                int intExperienciaQuinzenalAtual = objUsuarioAtual.ExperienciaQuinzenalUsuario;
                int intPosicaoAtual = objRanking.FirstOrDefault(u => u.IdUsuario == intIdUsuario)?.Posicao ?? -1;

                if (intPosicaoAtual == -1)
                {
                    intPosicaoAtual = await _context.Usuarios.CountAsync(u => u.ExperienciaQuinzenalUsuario > intExperienciaQuinzenalAtual) + 1;
                }

                return Ok(new
                {
                    TopUsuarios = objRanking,
                    PosicaoAtual = new { Nome = objUsuarioAtual.Nome, ExperienciaQuinzenal = intExperienciaQuinzenalAtual, Posicao = intPosicaoAtual }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Função para adicionar experiência para um usuário específico
        /// </summary>
        /// <param name="intIdUsuario">Obrigatório passar o Id do Usuário</param>
        /// <param name="objRequest">Obrigatório passar o a quantia de xp que será colocado para o usuário</param>
        /// <returns></returns>
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
            objUsuario.ExperienciaQuinzenalUsuario += objRequest.Experiencia;

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

            _context.Usuarios.Update(objUsuario);
            await _context.SaveChangesAsync();

            // Retornar a experiência e nível atualizados do usuário
            var objResponse = new UsuarioExperienciaResponse
            {
                NivelUsuario = objUsuario.NivelUsuario,
                ExperienciaUsuario = objUsuario.ExperienciaUsuario,
                ExperienciaQuinzenalUsuario = objUsuario.ExperienciaQuinzenalUsuario
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

            public Dictionary<TipoQuestao, int> TiposQuestoesRespondidas { get; set; } = new Dictionary<TipoQuestao, int>();
        }

        public class VerificarRespostaRequest
        {
            public int IdUsuario { get; set; }
            public int IdQuestao { get; set; }
            public int IdNivel { get; set; } 
            public int IdTrilha { get; set; } 
            public int IdOpcao { get; set; } 

            public string RespostaUsuario { get; set; } = string.Empty;
            public List<RespostaLacuna> RespostasLacunas { get; set; } = new List<RespostaLacuna>(); 

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
            public TipoQuestao TipoQuestao { get; set; }
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
            public int ExperienciaQuinzenalUsuario { get; set; }
        }

        public class UsuarioRankingResponse
        {
            public int IdUsuario { get; set; }
            public string? Nome { get; set; }
            public int ExperienciaQuinzenal { get; set; }
            public int Posicao { get; set; }
        }

        #endregion

        #region Judge0
        public class Judge0Response
        {
            [JsonPropertyName("stdout")]
            public string Stdout { get; set; } = String.Empty;

            [JsonPropertyName("time")]
            public string Time { get; set; } = String.Empty;

            [JsonPropertyName("memory")]
            public int Memory { get; set; }

            [JsonPropertyName("stderr")]
            public string Stderr { get; set; } = String.Empty;

            [JsonPropertyName("token")]
            public string Token { get; set; } = String.Empty;

            [JsonPropertyName("compile_output")]
            public string Compile_output { get; set; } = String.Empty;

            [JsonPropertyName("message")]
            public string Message { get; set; } = String.Empty;

            [JsonPropertyName("status")]
            public Status? Status { get; set; }  
        }

        public class Status
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; } = String.Empty;
        }

        #endregion
    }
}
