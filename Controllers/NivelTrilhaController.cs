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
    public class NiveisController : ControllerBase
    {
        private readonly BitBeakContext _context;

        public NiveisController(BitBeakContext context)
        {
            _context = context;
        }

        // GET: api/Niveis
        [HttpGet ("ObterListaNiveis")]
        public async Task<ActionResult<IEnumerable<ModelNivelTrilha>>> ObterListaNiveis()
        {
            return await _context.NiveisTrilha.ToListAsync();
        }

        // GET: api/Niveis/5
        [HttpGet("ListarDadosNivel/{intId}")]
        public async Task<ActionResult<ModelNivelTrilha>> ListarDadosNivel(int intId)
        {
            try
            {
                var objModelNivel = await _context.NiveisTrilha
                    .Include(nivel => nivel.Questoes)
                        .ThenInclude(questao => questao.Opcoes) 
                    .Include(nivel => nivel.Questoes)
                        .ThenInclude(questao => questao.Lacunas) 
                    .Include(nivel => nivel.Questoes)
                        .ThenInclude(questao => questao.CodeFill)
                    .FirstOrDefaultAsync(nivel => nivel.IdNivel == intId);

                if (objModelNivel == null)
                {
                    return NotFound();
                }

                return objModelNivel;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    // POST: api/Niveis
    [HttpPost("CriarNivel")]
        public async Task<ActionResult<ModelNivelTrilha>> CriarNivel(ModelNivelTrilha objModelNivel)
        {
            try
            {
                if (!_context.Trilhas.Any(t => t.IdTrilha == objModelNivel.IdTrilha))
                {
                    return NotFound($"Trilha com ID {objModelNivel.IdTrilha} não encontrada.");
                }

                _context.NiveisTrilha.Add(objModelNivel);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ListarDadosNivel), new { intId = objModelNivel.IdNivel }, objModelNivel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Niveis/5
        [HttpPut("EditarNivelTrilha/{intId}")]
        public async Task<IActionResult> EditarNivelTrilha(int intId, ModelNivelTrilha objModelNivel)
        {
            try
            {
                if (intId != objModelNivel.IdNivel)
                {
                    return BadRequest();
                }

                _context.Entry(objModelNivel).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NivelExists(intId))
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Niveis/5
        [HttpDelete("ExcluirNivelTrilha/{intId}")]
        public async Task<IActionResult> ExcluirNivelTrilha(int intId)
        {
            ModelNivelTrilha? objModelNivel = new();

            try
            {
                objModelNivel = await _context.NiveisTrilha.FindAsync(intId);
                if (objModelNivel == null)
                {
                    return NotFound();
                }

                _context.NiveisTrilha.Remove(objModelNivel);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool NivelExists(int intId)
        {
            return _context.NiveisTrilha.Any(e => e.IdNivel == intId);
        }
    }
}
