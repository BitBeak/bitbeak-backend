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

            // Carregar a trilha existente com os níveis e questões
            var trilhaExistente = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == id);

            if (trilhaExistente == null)
            {
                return NotFound();
            }

            // Atualizar os campos Nome e Descricao somente se fornecidos
            if (!string.IsNullOrEmpty(trilha.Nome))
            {
                trilhaExistente.Nome = trilha.Nome;
            }

            if (!string.IsNullOrEmpty(trilha.Descricao))
            {
                trilhaExistente.Descricao = trilha.Descricao;
            }

            // Atualizar os níveis
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
                    // Configurar a propriedade Trilha corretamente
                    nivel.Trilha = trilhaExistente;
                    trilhaExistente.Niveis.Add(nivel);
                }
            }

            _context.Entry(trilhaExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TrilhaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    // Adicionar log de erro detalhado para ajudar no diagnóstico
                    Console.WriteLine($"Erro de concorrência: {ex.Message}");
                    Console.WriteLine($"Detalhes do erro: {ex.InnerException?.Message}");
                    throw;
                }
            }

            return NoContent();
        }

        private bool TrilhaExists(int id)
        {
            return _context.Trilhas.Any(e => e.IdTrilha == id);
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
    }
}
