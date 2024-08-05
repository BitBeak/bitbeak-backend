using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BitBeakAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrilhasController : ControllerBase
    {
        private readonly BitBeakContext _context;

        public TrilhasController(BitBeakContext context)
        {
            _context = context;
        }

        #region Administradores

        // GET: api/Trilhas
        [HttpGet("ObterListaTrilhas")]
        public async Task<ActionResult<IEnumerable<ModelTrilha>>> ObterListaTrilhas()
        {
            return await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .ToListAsync();
        }

        // GET: api/Trilhas/5
        [HttpGet("ListarDadosTrilha/{intId}")]
        public async Task<ActionResult<ModelTrilha>> ListarDadosTrilha(int intId)
        {
            var objTrilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == intId);

            if (objTrilha == null)
            {
                return NotFound();
            }

            return objTrilha;
        }

        // POST: api/Trilhas
        [HttpPost("CriarTrilha")]
        public async Task<ActionResult<ModelTrilha>> CriarTrilha(ModelTrilha trilha)
        {
            _context.Trilhas.Add(trilha);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ListarDadosTrilha), new { intId = trilha.IdTrilha }, trilha);
        }

        // PUT: api/Trilhas/5
        [HttpPut("EditarTrilha/{intId}")]
        public async Task<IActionResult> EditarTrilha(int intId, ModelTrilha objTrilha)
        {
            if (intId != objTrilha.IdTrilha)
            {
                return BadRequest();
            }

            var objTrilhaExistente = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == intId);

            if (objTrilhaExistente == null)
            {
                return NotFound();
            }

            objTrilhaExistente.Nome = objTrilha.Nome ?? objTrilhaExistente.Nome;
            objTrilhaExistente.Descricao = objTrilha.Descricao ?? objTrilhaExistente.Descricao;

            foreach (ModelNivelTrilha objNivelTriha in objTrilha.Niveis)
            {
                var objNivelExistente = objTrilhaExistente.Niveis.FirstOrDefault(n => n.IdNivel == objNivelTriha.IdNivel);
                if (objNivelExistente != null)
                {
                    objNivelExistente.Nivel = objNivelTriha.Nivel;
                    objNivelExistente.Questoes = objNivelTriha.Questoes;
                }
                else
                {
                    objTrilhaExistente.Niveis.Add(objNivelTriha);
                }
            }

            _context.Entry(objTrilhaExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrilhaExists(intId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Trilhas/5
        [HttpDelete("ExcluirTrilha/{intId}")]
        public async Task<IActionResult> ExcluirTrilha(int intId)
        {
            var objTrilha = await _context.Trilhas.FindAsync(intId);
            if (objTrilha == null)
            {
                return NotFound();
            }

            _context.Trilhas.Remove(objTrilha);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Usuários

        // Função para entrar na trilha
        [HttpGet("EntrarNaTrilha/{intId}")]
        public async Task<ActionResult<ModelTrilha>> EntrarNaTrilha(int intId)
        {
            var objTrilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == intId);

            if (objTrilha == null)
            {
                return NotFound();
            }

            return objTrilha;
        }

        // Função para entrar no nível e obter uma questão aleatória
        [HttpGet("{intIdTrilha}/Niveis/{intIdNivel}/ObterQuestaoAleatoria/{intIdUsuario}")]
        public async Task<ActionResult<ModelQuestao>> ObterQuestaoAleatoria(int intIdTrilha, int intIdNivel, int intIdUsuario)
        {
            var objTrilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .ThenInclude(q => q.Opcoes)
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .ThenInclude(q => q.Lacunas)
                .FirstOrDefaultAsync(t => t.IdTrilha == intIdTrilha);

            if (objTrilha == null)
            {
                return NotFound();
            }

            var objNivelTrilha = objTrilha.Niveis.FirstOrDefault(n => n.Nivel == intIdNivel);

            if (objNivelTrilha == null || !objNivelTrilha.Questoes.Any())
            {
                return NotFound();
            }

            var objQuestoesRespondidas = await _context.QuestoesRespondidas
                .Where(qr => qr.IdUsuario == intIdUsuario)
                .Select(qr => qr.IdQuestao)
                .ToListAsync();

            var objQuestaoAleatoria = objNivelTrilha.Questoes
                .Where(q => !objQuestoesRespondidas.Contains(q.IdQuestao))
                .OrderBy(q => Guid.NewGuid())
                .FirstOrDefault();

            if (objQuestaoAleatoria == null)
            {
                return NotFound();
            }

            return objQuestaoAleatoria;
        }

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
            if (blnAcertou)
            {
                objTrilhaProgresso.ExperienciaUsuario++;
            }
            else
            {
                objTrilhaProgresso.Erros++;
            }

            // Adiciona a questão respondida à tabela de questões respondidas
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


        // Função para dar experiência a um usuário específico
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

        #endregion

        private bool TrilhaExists(int intId)
        {
            return _context.Trilhas.Any(e => e.IdTrilha == intId);
        }
    }

    #region Classes Necessárias

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
