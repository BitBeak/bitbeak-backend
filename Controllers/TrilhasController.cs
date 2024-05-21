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
        public async Task<ActionResult<ModelTrilha>> GetTrilha(int intId)
        {
            ModelTrilha? objModelTrilha = new();
            objModelTrilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == intId);

            if (objModelTrilha == null)
            {
                return NotFound();
            }

            return objModelTrilha;
        }

        // POST: api/Trilhas
        [HttpPost]
        public async Task<ActionResult<ModelTrilha>> PostTrilha(ModelTrilha objModelTrilha)
        {
            _context.Trilhas.Add(objModelTrilha);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrilha), new { id = objModelTrilha.IdTrilha }, objModelTrilha);
        }

        // PUT: api/Trilhas/5
        [HttpPut("{intId}")]
        public async Task<IActionResult> PutTrilha(int intId, ModelTrilha objModelTrilha)
        {
            ModelTrilha? objTrilhaExistente = new();

            try
            {
                if (intId != objModelTrilha.IdTrilha)
                {
                    return BadRequest();
                }

                // Carregar a trilha existente com os níveis e questões
                objTrilhaExistente = await _context.Trilhas
                    .Include(t => t.Niveis)
                    .ThenInclude(n => n.Questoes)
                    .FirstOrDefaultAsync(t => t.IdTrilha == intId);

                if (objTrilhaExistente == null)
                {
                    return NotFound();
                }

                // Atualizar os campos Nome e Descricao somente se fornecidos
                if (!string.IsNullOrEmpty(objModelTrilha.Nome))
                {
                    objTrilhaExistente.Nome = objModelTrilha.Nome;
                }

                if (!string.IsNullOrEmpty(objModelTrilha.Descricao))
                {
                    objTrilhaExistente.Descricao = objModelTrilha.Descricao;
                }

                // Atualizar os níveis
                foreach (ModelNivelTrilha objNivel in objModelTrilha.Niveis)
                {
                    ModelNivelTrilha? objNivelExistente = new();

                    objNivelExistente = objTrilhaExistente.Niveis.FirstOrDefault(n => n.IdNivel == objNivel.IdNivel);

                    if (objNivelExistente != null)
                    {
                        objNivelExistente.Nivel = objNivel.Nivel;
                        objNivelExistente.Questoes = objNivel.Questoes;
                    }
                    else
                    {
                        // Configurar a propriedade Trilha corretamente
                        objNivel.Trilha = objTrilhaExistente;
                        objTrilhaExistente.Niveis.Add(objNivel);
                    }
                }

                _context.Entry(objTrilhaExistente).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!TrilhaExists(intId))
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
            catch (Exception  ex)
            {
                return BadRequest(ex.Message);
            }
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

        private bool TrilhaExists(int id)
        {
            return _context.Trilhas.Any(e => e.IdTrilha == id);
        }
    }
}
