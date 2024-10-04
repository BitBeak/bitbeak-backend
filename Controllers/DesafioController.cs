using BitBeakAPI.Models;
using BitBeakAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static BitBeakAPI.Controllers.JogoController;
using System.Text.Json;
using System.Text;
using DotNetEnv;
using System.Text.Json.Serialization;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesafioController : ControllerBase
    {
        private readonly BitBeakContext _context;
        private readonly QuestaoService _questaoService;
        private readonly MissaoService _missaoService;
        private readonly string _strApiKey;

        public DesafioController(BitBeakContext context, QuestaoService questaoService, MissaoService objMissaoService)
        {
            _context = context;
            _questaoService = questaoService;
            _missaoService = objMissaoService;

            Env.Load();  // Carrega as variáveis de ambiente do arquivo .env
            _strApiKey = Environment.GetEnvironmentVariable("RAPIDAPI_KEY")!;
        }

        /// <summary>
        /// Função para iniciar um desafio
        /// </summary>
        /// <param name="objDesafioRequest"></param>
        /// <returns></returns>
        [HttpPost("IniciarDesafio")]
        public async Task<ActionResult> IniciarDesafio(IniciarDesafioRequest objDesafioRequest)
        {
            try
            {
                var objDesafiante = await _context.Usuarios.FindAsync(objDesafioRequest.IdDesafiante);
                var objDesafiado = await _context.Usuarios.FindAsync(objDesafioRequest.IdDesafiado);

                if (objDesafiante == null)
                {
                    return NotFound($"Desafiante com ID {objDesafioRequest.IdDesafiante} não encontrado.");
                }

                if (objDesafiado == null)
                {
                    return NotFound($"Desafiado com ID {objDesafioRequest.IdDesafiado} não encontrado.");
                }

                var objDesafioExistente = await _context.Desafios
                    .FirstOrDefaultAsync(d =>
                        ((d.IdDesafiante == objDesafioRequest.IdDesafiante && d.IdDesafiado == objDesafioRequest.IdDesafiado) ||
                        (d.IdDesafiante == objDesafioRequest.IdDesafiado && d.IdDesafiado == objDesafioRequest.IdDesafiante))
                        && !d.Finalizado);

                if (objDesafioExistente != null)
                {
                    return BadRequest("Já existe um desafio em andamento entre esses usuários.");
                }

                var objQuestoesRespondidas = new HashSet<int>();
                var objTiposQuestoesRespondidas = new Dictionary<TipoQuestao, int>();
                int intContadorAcertos = 0;
                int intContadorErros = 0;

                var objPrimeiraQuestao = await ObterProximaQuestao(objDesafioRequest.IdTrilha, 1, objQuestoesRespondidas, objTiposQuestoesRespondidas);

                if (objPrimeiraQuestao is ModelQuestao objDadosQuestao)
                {
                    ModelDesafio objDesafio = new()
                    {
                        IdDesafiante = objDesafioRequest.IdDesafiante,
                        IdDesafiado = objDesafioRequest.IdDesafiado,
                        IdTrilha = objDesafioRequest.IdTrilha,
                        NivelDesafiante = 1,
                        NivelDesafiado = 1,
                        DesafianteJogando = true,
                    };

                    _context.Desafios.Add(objDesafio);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        IdDesafio = objDesafio.IdDesafio,
                        Questao = objDadosQuestao,
                        IdUsuario = objDesafioRequest.IdDesafiante,
                        QuestoesRespondidas = objQuestoesRespondidas,
                        TiposQuestoesRespondidas = objTiposQuestoesRespondidas,
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
        /// Função para responder questão (desafio)
        /// </summary>
        /// <param name="objRequest"></param>
        /// <returns></returns>
        [HttpPost("ResponderQuestaoDesafio")]
        public async Task<ActionResult> ResponderQuestaoDesafio(ResponderQuestaoDesafioRequest objRequest)
        {
            var objDesafio = await _context.Desafios
                .FirstOrDefaultAsync(d => d.IdDesafio == objRequest.IdDesafio);

           

            try
            {

                if (objDesafio == null)
                {
                    return NotFound("Desafio não encontrado.");
                }

                if (objDesafio.StatusAceiteDesafio && objRequest.IdUsuario == objDesafio.IdDesafiado)
                {
                    objDesafio.StatusAceiteDesafio = false;
                    await _context.SaveChangesAsync();
                }

                if ((objDesafio.DesafianteJogando && objRequest.IdUsuario != objDesafio.IdDesafiante) ||
                    (!objDesafio.DesafianteJogando && objRequest.IdUsuario != objDesafio.IdDesafiado))
                {
                    return BadRequest("Não é a sua vez de jogar.");
                }

                // Verificar qual é o nível do jogador atual (Desafiante ou Desafiado)
                int intJogadorAtualNivel = objDesafio!.DesafianteJogando ? objDesafio.NivelDesafiante : objDesafio.NivelDesafiado;

                var objRespostaRequest = new VerificarRespostaDesafioRequest
                {
                    IdQuestao = objRequest.IdQuestaoAleatoria,
                    IdOpcao = objRequest.IdOpcaoEscolhidaUsuario,
                    IdUsuario = objRequest.IdUsuario,
                    IdTrilha = objDesafio.IdTrilha,
                    IdNivel = intJogadorAtualNivel,
                    RespostaUsuario = objRequest.RespostaUsuario,
                    RespostasLacunas = objRequest.RespostasLacunas.Select(objLacuna => new VerificarRespostaDesafioRequest.RespostaLacunaDesafio
                    { IdLacuna = objLacuna.IdLacuna, RespostaColunaA = objLacuna.RespostaColunaA, RespostaColunaB = objLacuna.RespostaColunaB }
                                                                         ).ToList(),
                };

                // Verificar se o jogador acertou ou errou a questão
                var objResultadoResposta = await VerificarResposta(objRespostaRequest);

                if (objResultadoResposta.Result is OkObjectResult okResultResposta &&
                    okResultResposta.Value is VerificarRespostaDesafioResponse objVerificarResposta)
                {
                    if (objVerificarResposta.Acertou)
                    {
                        objRequest.ContadorAcertos++;

                        int intIdMissaoQuestao = await _missaoService.BuscarMissaoAtiva(objRequest.IdUsuario, TipoMissao.Questao);

                        await _missaoService.AtualizarProgressoMissao(objRequest.IdUsuario, intIdMissaoQuestao, TipoMissao.Questao, 1);
                    }
                    else
                    {
                        objRequest.ContadorErros++;
                    }

                    if (objRequest.QuestoesRespondidas == null)
                    {
                        objRequest.QuestoesRespondidas = new HashSet<int>();
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

                    // Verificar se é necessário enviar a questão especial para o jogador
                    if (objRequest.ContadorAcertos == 3)
                    {
                        var objPerguntaEspecialResponse = await ObterQuestaoEspecial(objDesafio.IdTrilha, intJogadorAtualNivel);

                        if (objPerguntaEspecialResponse.Result is OkObjectResult okResult && okResult.Value is ModelQuestao objPerguntaEspecial)
                        {
                            objRequest.IdQuestaoAleatoria = objPerguntaEspecial.IdQuestao;

                            return Ok(new
                            {
                                IdDesafio = objDesafio.IdDesafio,
                                PerguntaEspecial = objPerguntaEspecial,
                                Questao = objVerificarResposta,
                                ProximaFase = "Questão Especial",
                                ContadorAcertos = objRequest.ContadorAcertos,
                                ContadorErros = objRequest.ContadorErros
                            });
                        }
                    }
                }
                else
                {
                    return BadRequest("Erro ao verificar resposta.");
                }

                //Verificar se Encerrou a vez do jogador
                if (objRequest.ContadorErros == 1)
                {
                    objDesafio.UltimaAtualizacao = DateTime.Now;
                    objDesafio.DesafianteJogando = !objDesafio.DesafianteJogando;

                    await _context.SaveChangesAsync();
                    return Ok("Turno encerrado, agora é a vez do outro jogador.");
                }
                else if (objRequest.ContadorAcertos == 4)
                {
                    if (objDesafio.DesafianteJogando)
                    {
                        objDesafio.InsigniasDesafiante++; 
                        objDesafio.NivelDesafiante++; 
                    }
                    else
                    {
                        objDesafio.InsigniasDesafiado++; 
                        objDesafio.NivelDesafiado++; 
                    }

                    // Verificar se a um vencedor no desafio
                    if (objDesafio.InsigniasDesafiante == 5 || objDesafio.InsigniasDesafiado == 5)
                    {
                        int intIdVencedor = objDesafio.InsigniasDesafiante == 5 ? objDesafio.IdDesafiante : objDesafio.IdDesafiado;

                        ModelHistoricoDesafio objHistoricoConfronto = new()
                        {
                            IdDesafio = objDesafio.IdDesafio,
                            IdDesafiante = objDesafio.IdDesafiante,
                            IdDesafiado = objDesafio.IdDesafiado,
                            IdVencedor = intIdVencedor
                        };

                        if (objHistoricoConfronto.IdVencedor == objRequest.IdUsuario)
                        {
                            int intIdMissaoDesafio = await _missaoService.BuscarMissaoAtiva(objRequest.IdUsuario, TipoMissao.Desafio);

                            await _missaoService.AtualizarProgressoMissao(objRequest.IdUsuario, intIdMissaoDesafio, TipoMissao.Desafio, 1);
                        }

                        _context.HistoricoDesafios.Add(objHistoricoConfronto);
                        objDesafio.Finalizado = true;

                        await _context.SaveChangesAsync();

                        return Ok("Jogo finalizado! O jogador ganhou todas as insígnias.");
                    }

                    objDesafio.UltimaAtualizacao = DateTime.Now;
                    objDesafio.DesafianteJogando = !objDesafio.DesafianteJogando;

                    await _context.SaveChangesAsync();
                    return Ok("Insígnia conquistada e turno encerrado, agora é a vez do outro jogador.");
                }

                var objProximaQuestao = await ObterProximaQuestao(objDesafio.IdTrilha,
                                                                  intJogadorAtualNivel,
                                                                  objRequest.QuestoesRespondidas!,
                                                                  objRequest.TiposQuestoesRespondidas);

                if (objProximaQuestao is ModelQuestao objDadosQuestao)
                {
                    return Ok(new
                    {
                        IdDesafio = objDesafio.IdDesafio,
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Função para Listar os desafios em aberto
        /// </summary>
        /// <param name="intIdUsuario"></param>
        /// <returns></returns>
        [HttpGet("ListarDesafiosAbertos/{intIdUsuario}")]
        public async Task<ActionResult> ListarDesafiosAbertos(int intIdUsuario)
        {
            var objDesafios = await _context.Desafios
                .Where(d => (d.IdDesafiante == intIdUsuario || d.IdDesafiado == intIdUsuario) && !d.Finalizado)
                .Include(d => d.Desafiante)
                .Include(d => d.Desafiado)
                .Include(d => d.Trilha)  
                .ToListAsync();

            if (!objDesafios.Any())
            {
                return NotFound("Nenhum desafio em aberto encontrado.");
            }

            var objListaDesafios = objDesafios.Select(d => new
            {
                IdDesafio = d.IdDesafio,
                NomeDesafiante = d.Desafiante!.Nome,
                NomeDesafiado = d.Desafiado!.Nome,
                NomeTrilha = d.Trilha!.Nome,
                NivelDesafiante = d.NivelDesafiante,  
                NivelDesafiado = d.NivelDesafiado,
                InsigniaDesafiante = d.InsigniasDesafiante,
                InsigniasDesafiado = d.InsigniasDesafiado,
                JogadorAtual = d.DesafianteJogando ? d.Desafiante!.Nome : d.Desafiado!.Nome,
                UltimaAtualizacao = d.UltimaAtualizacao
            }).ToList();

            return Ok(objListaDesafios);
        }

        /// <summary>
        /// Função para verificar se é a vez do usuário jogar
        /// </summary>
        /// <param name="idDesafio"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpGet("VerificarVez/{idDesafio}/{idUsuario}")]
        public async Task<ActionResult> VerificarVez(int idDesafio, int idUsuario)
        {
            try
            {
                var objDesafio = await _context.Desafios
                .Include(d => d.Desafiante)
                .Include(d => d.Desafiado)
                .FirstOrDefaultAsync(d => d.IdDesafio == idDesafio);

                if (objDesafio == null)
                {
                    return NotFound("Desafio não encontrado.");
                }

                if (objDesafio.IdDesafiante != idUsuario && objDesafio.IdDesafiado != idUsuario)
                {
                    return BadRequest("Usuário não faz parte deste desafio.");
                }

                if (objDesafio.DesafianteJogando && objDesafio.IdDesafiante == idUsuario)
                {
                    return Ok(new
                    {
                        Mensagem = "É sua vez de jogar.",
                        NivelAtual = objDesafio.NivelDesafiante
                    });
                }
                else if (!objDesafio.DesafianteJogando && objDesafio.IdDesafiado == idUsuario)
                {
                    return Ok(new
                    {
                        Mensagem = "É sua vez de jogar.",
                        NivelAtual = objDesafio.NivelDesafiado
                    });
                }
                else
                {
                    var strNomeOutroJogador = objDesafio.DesafianteJogando ? objDesafio.Desafiante!.Nome : objDesafio.Desafiado!.Nome;

                    return Ok(new
                    {
                        Mensagem = $"Ainda não é sua vez. Aguarde {strNomeOutroJogador} jogar.",
                        NivelAtual = objDesafio.DesafianteJogando ? objDesafio.NivelDesafiante : objDesafio.NivelDesafiado
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Função para entrar no desafio após ele já ter sido iniciado
        /// </summary>
        /// <param name="idDesafio"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpGet("EntrarNoDesafio/{idDesafio}/{idUsuario}")]
        public async Task<ActionResult> EntrarNoDesafio(int idDesafio, int idUsuario)
        {
            var objDesafio = await _context.Desafios
                .Include(d => d.Desafiante)
                .Include(d => d.Desafiado)
                .FirstOrDefaultAsync(d => d.IdDesafio == idDesafio);

            if (objDesafio == null)
            {
                return NotFound("Desafio não encontrado.");
            }

            if ((objDesafio.DesafianteJogando && objDesafio.IdDesafiante != idUsuario) ||
                (!objDesafio.DesafianteJogando && objDesafio.IdDesafiado != idUsuario))
            {
                return BadRequest("Não é a sua vez de jogar.");
            }

            // Determinar o nível atual com base no jogador (Desafiante ou Desafiado)
            var intNivelAtual = objDesafio.IdDesafiante == idUsuario ? objDesafio.NivelDesafiante : objDesafio.NivelDesafiado;

            var objQuestoesRespondidas = new HashSet<int>();
            var objTiposQuestoesRespondidas = new Dictionary<TipoQuestao, int>();

            var objPrimeiraQuestao = await ObterProximaQuestao(objDesafio.IdTrilha, intNivelAtual, objQuestoesRespondidas, objTiposQuestoesRespondidas);

            if (objPrimeiraQuestao is ModelQuestao objDadosQuestao)
            {
                return Ok(new
                {
                    IdDesafio = objDesafio.IdDesafio, 
                    Questao = objDadosQuestao,
                    QuestoesRespondidas = objQuestoesRespondidas,
                    TiposQuestoesRespondidas = objTiposQuestoesRespondidas,
                    NivelAtual = intNivelAtual,
                    Mensagem = "Você pode começar a jogar."
                });
            }
            else
            {
                return NotFound("Nenhuma questão encontrada para o nível atual.");
            }
        }

        [HttpGet("ListarHistoricoDesafios/{intIdJogador1}/{intIdJogador2}")]
        public async Task<ActionResult> ListarHistoricoDesafios(int intIdJogador1, int intIdJogador2)
        {
            var objHistoricoConfrontos = await _context.HistoricoDesafios
                .Where(h => (h.IdDesafiante == intIdJogador1 && h.IdDesafiado == intIdJogador2) ||
                            (h.IdDesafiante == intIdJogador2 && h.IdDesafiado == intIdJogador1))
                .Include(h => h.Desafiante)
                .Include(h => h.Desafiado)
                .ToListAsync();

            if (!objHistoricoConfrontos.Any())
            {
                return NotFound("Nenhum confronto encontrado entre esses dois jogadores.");
            }

            string strNomeJogador1 = objHistoricoConfrontos.First().IdDesafiante == intIdJogador1
                ? objHistoricoConfrontos.First().Desafiante!.Nome
                : objHistoricoConfrontos.First().Desafiado!.Nome;

            string strNomeJogador2 = objHistoricoConfrontos.First().IdDesafiante == intIdJogador2
                ? objHistoricoConfrontos.First().Desafiante!.Nome
                : objHistoricoConfrontos.First().Desafiado!.Nome;

            int intVitoriasJogador1 = objHistoricoConfrontos.Count(h => h.IdVencedor == intIdJogador1);
            int intVitoriasJogador2 = objHistoricoConfrontos.Count(h => h.IdVencedor == intIdJogador2);

            var objListaHistorico = objHistoricoConfrontos.Select(h => new
            {
                IdConfronto = h.IdHistoricoConfronto,
                Desafiante = h.Desafiante!.Nome,
                Desafiado = h.Desafiado!.Nome,
                Vencedor = h.IdVencedor == h.IdDesafiante ? h.Desafiante!.Nome : h.Desafiado!.Nome,
                DataConfronto = h.DataConfronto
            }).ToList();

            return Ok(new
            {
                Historico = objListaHistorico,
                Vitorias = new Dictionary<string, int>
                {
                    { strNomeJogador1, intVitoriasJogador1 },
                    { strNomeJogador2, intVitoriasJogador2 }
                }
            });
        }

        /// <summary>
        /// Função para listar os desafios pendentes de aceite do usuário
        /// </summary>
        /// <param name="intIdUsuario"></param>
        /// <returns></returns>
        [HttpGet("ListarDesafiosPendentes/{intIdUsuario}")]
        public async Task<ActionResult> ListarDesafiosPendentes(int intIdUsuario)
        {
            var objDesafiosPendentes = await _context.Desafios
                .Where(d => d.IdDesafiado == intIdUsuario && d.StatusAceiteDesafio && !d.Finalizado)
                .Include(d => d.Desafiante)
                .Include(d => d.Trilha)
                .ToListAsync();

            if (!objDesafiosPendentes.Any())
            {
                return NotFound("Nenhum desafio pendente para esse usuário.");
            }

            var objListaDesafios = objDesafiosPendentes.Select(d => new
            {
                IdDesafio = d.IdDesafio,
                NomeDesafiante = d.Desafiante!.Nome,
                Trilha = d.Trilha != null ? d.Trilha.Nome : "Trilha não encontrada",
                DataCriacao = d.DataCriacao
            }).ToList();

            return Ok(objListaDesafios);
        }

        /// <summary>
        /// Função parar negar desafio
        /// </summary>
        /// <param name="intIdDesafio"></param>
        /// <returns></returns>
        [HttpPost("NegarDesafio/{intIdDesafio}")]
        public async Task<ActionResult> NegarDesafio(int intIdDesafio)
        {

            var desafio = await _context.Desafios.FirstOrDefaultAsync(d => d.IdDesafio == intIdDesafio && d.StatusAceiteDesafio);

            if (desafio == null)
            {
                return NotFound("Desafio não encontrado ou já foi aceito.");
            }

            desafio.Finalizado = true;

            await _context.SaveChangesAsync();

            return Ok("Desafio negado com sucesso.");
        }

        #region Obter Questão
        [NonAction]
        private async Task<ModelQuestao> ObterProximaQuestao(int idTrilha, int idNivelTrilha, HashSet<int> objQuestoesRespondidas, Dictionary<TipoQuestao, int> tiposQuestoesRespondidas)
        {
            var objResultadoAleatoria = await ObterIdQuestaoAleatoria(idTrilha, idNivelTrilha);

            if (objResultadoAleatoria.Result is OkObjectResult okResult && okResult.Value is int idQuestaoAleatoria)
            {
                if (objQuestoesRespondidas.Contains(idQuestaoAleatoria))
                {
                    return await ObterProximaQuestao(idTrilha, idNivelTrilha, objQuestoesRespondidas, tiposQuestoesRespondidas);
                }

                // Buscar os dados da questão obtida aleatoriamente
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
                    return null!; 
                }
            }
            else
            {
                return null!; 
            }
        }

        [HttpGet("{intIdTrilha}/Niveis/{intIdNivel}/ObterIdQuestaoAleatoria")]
        public async Task<ActionResult<int>> ObterIdQuestaoAleatoria(int intIdTrilha, int intIdNivel)
        {
            var objNivelTrilha = await _context.Trilhas
                                        .Where(t => t.IdTrilha == intIdTrilha)
                                        .SelectMany(t => t.Niveis)
                                        .Include(n => n.Questoes) 
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

        [HttpGet("{intIdTrilha}/Niveis/{intIdNivel}/ObterQuestaoEspecial")]
        public async Task<ActionResult<ModelQuestao>> ObterQuestaoEspecial(int intIdTrilha, int intIdNivel)
        {
            var objQuestaoEspecial = await _context.Questoes
                .Where(q => q.IdNivel == intIdNivel && q.Nivel!.IdTrilha == intIdTrilha && q.Tipo == TipoQuestao.Desafio)
                .OrderBy(q => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (objQuestaoEspecial != null)
            {
                var objResultadoDados = await _questaoService.ListarDadosQuestao(objQuestaoEspecial.IdQuestao); 

                if (objResultadoDados != null)
                {
                    return Ok(objResultadoDados);  
                }
                else
                {
                    return NotFound("Não foi possível obter os dados da questão especial.");
                }
            }
            else
            {
                return NotFound("Nenhuma questão especial encontrada para o nível e trilha especificados.");
            }
        }
        #endregion

        #region Verificar Resposta

        [NonAction]
        public async Task<ActionResult<VerificarRespostaDesafioResponse>> VerificarResposta(VerificarRespostaDesafioRequest objRequest)
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
                case TipoQuestao.Desafio:
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
                        blnAcertou = false; 
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

            return Ok(new VerificarRespostaDesafioResponse
            {
                Acertou = blnAcertou,
                NivelAtual = objQuestao.Nivel!.Nivel,
                TipoQuestao = objQuestao.Tipo // Atribuindo o tipo da questão
            });
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
                        Judge0ResponseDesafio objResult = JsonSerializer.Deserialize<Judge0ResponseDesafio>(strBody)!;

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
        #endregion

        #region Classes necessárias
        public class ResponderQuestaoDesafioRequest
        {
            public int IdDesafio {  get; set; }
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

        public class VerificarRespostaDesafioRequest
        {
            public int IdDesafio { get; set; }
            public int IdUsuario { get; set; }
            public int IdQuestao { get; set; }
            public int IdNivel { get; set; }
            public int IdTrilha { get; set; }
            public int IdOpcao { get; set; }

            public string RespostaUsuario { get; set; } = string.Empty;
            public List<RespostaLacunaDesafio> RespostasLacunas { get; set; } = new List<RespostaLacunaDesafio>();

            // Classe interna para respostas de lacuna
            public class RespostaLacunaDesafio
            {
                public int IdLacuna { get; set; }
                public string? RespostaColunaA { get; set; }
                public string? RespostaColunaB { get; set; }
            }
        }

        public class RespostaLacunaDesafio
        {
            public int IdLacuna { get; set; }
            public string? RespostaColunaA { get; set; }
            public string? RespostaColunaB { get; set; }
        }

        public class VerificarRespostaDesafioResponse
        {
            public int IdDesafio { get; set; }
            public bool Acertou { get; set; }
            public int NivelAtual { get; set; }
            public int ExperienciaAtual { get; set; }
            public int PenasAtuais { get; set; }
            public TipoQuestao TipoQuestao { get; set; }
        }


        public class IniciarDesafioRequest
        {
            [Required]
            public int IdDesafiante { get; set; } 

            [Required]
            public int IdDesafiado { get; set; } 

            [Required]
            public int IdTrilha { get; set; }

            public HashSet<int> QuestoesRespondidas { get; set; } = new HashSet<int>();

            public Dictionary<TipoQuestao, int> TiposQuestoesRespondidas { get; set; } = new Dictionary<TipoQuestao, int>();
        }
        #endregion

        #region Judge0
        public class Judge0ResponseDesafio
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
