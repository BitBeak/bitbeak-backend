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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelTrilha>>> GetTrilhas()
        {
            return await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .ToListAsync();
        }

        // GET: api/Trilhas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModelTrilha>> GetTrilha(int id)
        {
            var trilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == id);

            if (trilha == null)
            {
                return NotFound();
            }

            return trilha;
        }

        // POST: api/Trilhas
        [HttpPost]
        public async Task<ActionResult<ModelTrilha>> PostTrilha(ModelTrilha trilha)
        {
            _context.Trilhas.Add(trilha);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrilha), new { id = trilha.IdTrilha }, trilha);
        }

        // PUT: api/Trilhas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrilha(int id, ModelTrilha trilha)
        {
            if (id != trilha.IdTrilha)
            {
                return BadRequest();
            }

            var trilhaExistente = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == id);

            if (trilhaExistente == null)
            {
                return NotFound();
            }

            trilhaExistente.Nome = trilha.Nome ?? trilhaExistente.Nome;
            trilhaExistente.Descricao = trilha.Descricao ?? trilhaExistente.Descricao;

            foreach (var nivel in trilha.Niveis)
            {
                var nivelExistente = trilhaExistente.Niveis.FirstOrDefault(n => n.IdNivel == nivel.IdNivel);
                if (nivelExistente != null)
                {
                    nivelExistente.Nivel = nivel.Nivel;
                    nivelExistente.Questoes = nivel.Questoes;
                }
                else
                {
                    nivel.Trilha = trilhaExistente;
                    trilhaExistente.Niveis.Add(nivel);
                }
            }

            _context.Entry(trilhaExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrilhaExists(id))
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrilha(int id)
        {
            var trilha = await _context.Trilhas.FindAsync(id);
            if (trilha == null)
            {
                return NotFound();
            }

            _context.Trilhas.Remove(trilha);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Usuários

        // Função para entrar na trilha
        [HttpGet("{id}/EntrarNaTrilha")]
        public async Task<ActionResult<ModelTrilha>> EntrarNaTrilha(int id)
        {
            var trilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == id);

            if (trilha == null)
            {
                return NotFound();
            }

            return trilha;
        }

        // Função para entrar no nível e obter uma questão aleatória
        [HttpGet("Trilhas/{idTrilha}/Niveis/{nivel}/ObterQuestaoAleatoria")]
        public async Task<ActionResult<ModelQuestao>> ObterQuestaoAleatoria(int idTrilha, int nivel)
        {
            var trilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .ThenInclude(q => q.Opcoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == idTrilha);

            if (trilha == null)
            {
                return NotFound();
            }

            var nivelTrilha = trilha.Niveis.FirstOrDefault(n => n.Nivel == nivel);

            if (nivelTrilha == null || !nivelTrilha.Questoes.Any())
            {
                return NotFound();
            }

            var questaoAleatoria = nivelTrilha.Questoes
                .OrderBy(q => Guid.NewGuid())
                .FirstOrDefault();

            if (questaoAleatoria == null)
            {
                return NotFound();
            }

            return questaoAleatoria;
        }

        // Função para verificar a resposta do usuário
        [HttpPost("VerificarResposta")]
        public async Task<ActionResult<VerificarRespostaResponse>> VerificarResposta(VerificarRespostaRequest request)
        {
            var questao = await _context.Questoes
                .Include(q => q.Opcoes)
                .FirstOrDefaultAsync(q => q.IdQuestao == request.IdQuestao);

            if (questao == null)
            {
                return NotFound("Questão não encontrada.");
            }

            var opcaoEscolhida = questao.Opcoes.FirstOrDefault(o => o.IdOpcao == request.IdOpcao);

            if (opcaoEscolhida == null)
            {
                return NotFound("Opção de resposta não encontrada.");
            }

            var usuario = await _context.Usuarios.FindAsync(request.IdUsuario);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var trilhaProgresso = await _context.UsuarioTrilhaProgresso
                .FirstOrDefaultAsync(p => p.IdUsuario == request.IdUsuario && p.IdTrilha == questao.Nivel!.IdTrilha);

            if (trilhaProgresso == null)
            {
                trilhaProgresso = new ModelUsuarioTrilhaProgresso
                {
                    IdUsuario = request.IdUsuario,
                    IdTrilha = questao.Nivel!.IdTrilha,
                    NivelUsuario = questao.Nivel.Nivel,
                    ExperienciaUsuario = 0,
                    Penas = 0,
                    Erros = 0
                };
                _context.UsuarioTrilhaProgresso.Add(trilhaProgresso);
                await _context.SaveChangesAsync();
            }

            bool acertou = opcaoEscolhida.Correta;
            if (acertou)
            {
                trilhaProgresso.ExperienciaUsuario++;
            }
            else
            {
                trilhaProgresso.Erros++;
            }

            if (trilhaProgresso.ExperienciaUsuario >= 5)
            {
                // Concluir o nível
                trilhaProgresso.NivelUsuario++;
                trilhaProgresso.ExperienciaUsuario = 0; // Resetar a experiência para o próximo nível
                usuario.Penas += 10;

                // Atualizar a experiência baseada nos erros
                int expGanha = trilhaProgresso.Erros switch
                {
                    0 => 50,
                    1 => 45,
                    2 => 40,
                    3 => 30,
                    4 => 20,
                    _ => 10
                };

                usuario.ExperienciaUsuario += expGanha;
                trilhaProgresso.Erros = 0; // Resetar os erros após a conclusão do nível

                // Verificar se o usuário pode upar de nível
                while (true)
                {
                    var nivelUsuario = await _context.NiveisUsuario
                        .FirstOrDefaultAsync(nu => nu.NivelUsuario == usuario.NivelUsuario);

                    if (nivelUsuario == null || usuario.ExperienciaUsuario < nivelUsuario.ExperienciaNecessaria)
                    {
                        break;
                    }

                    // Upar de nível
                    usuario.NivelUsuario++;
                    usuario.ExperienciaUsuario -= nivelUsuario.ExperienciaNecessaria; // Reduzir a experiência acumulada
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new VerificarRespostaResponse
            {
                Acertou = acertou,
                NivelAtual = trilhaProgresso.NivelUsuario,
                ExperienciaAtual = trilhaProgresso.ExperienciaUsuario,
                PenasAtuais = usuario.Penas
            });
        }

        // Função para dar experiência a um usuário específico
        [HttpPost("{usuarioId}/AdicionarExperiencia")]
        public async Task<IActionResult> AdicionarExperiencia(int usuarioId, [FromBody] AdicionarExperienciaRequest request)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            // Adicionar a experiência ao usuário
            usuario.ExperienciaUsuario += request.Experiencia;

            // Verificar se o usuário pode upar de nível
            while (true)
            {
                var nivelUsuario = await _context.NiveisUsuario
                    .FirstOrDefaultAsync(nu => nu.NivelUsuario == usuario.NivelUsuario);

                if (nivelUsuario == null || usuario.ExperienciaUsuario < nivelUsuario.ExperienciaNecessaria)
                {
                    break;
                }

                // Upar de nível
                usuario.NivelUsuario++;
                usuario.ExperienciaUsuario -= nivelUsuario.ExperienciaNecessaria; // Reduzir a experiência acumulada
            }

            await _context.SaveChangesAsync();

            // Retornar a experiência e nível atualizados do usuário
            var response = new UsuarioExperienciaResponse
            {
                NivelUsuario = usuario.NivelUsuario,
                ExperienciaUsuario = usuario.ExperienciaUsuario
            };

            return Ok(response);
        }

        #endregion

        private bool TrilhaExists(int id)
        {
            return _context.Trilhas.Any(e => e.IdTrilha == id);
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
