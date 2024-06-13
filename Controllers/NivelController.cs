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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelNivelTrilha>>> GetNiveis()
        {
            return await _context.NiveisTrilha.ToListAsync();
        }

        // GET: api/Niveis/5
        [HttpGet("{intId}")]
        public async Task<ActionResult<ModelNivelTrilha>> GetNivel(int intId)
        {
            ModelNivelTrilha? objModelNivel = new();

            try
            {
                objModelNivel = await _context.NiveisTrilha.FindAsync(intId);

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
        [HttpPost]
        public async Task<ActionResult<ModelNivelTrilha>> PostNivel(ModelNivelTrilha objModelNivel)
        {
            try
            {
                _context.NiveisTrilha.Add(objModelNivel);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetNivel), new { id = objModelNivel.IdNivel }, objModelNivel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Niveis/5
        [HttpPut("{intId}")]
        public async Task<IActionResult> PutNivel(int intId, ModelNivelTrilha objModelNivel)
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
        [HttpDelete("{intId}")]
        public async Task<IActionResult> DeleteNivel(int intId)
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

        // PATCH: api/Niveis/5/AdicionarNivelEmTrilha/3
        [HttpPatch("{intNivelId}/AdicionarNivelEmTrilha/{idTrilhaId}")]
        public async Task<IActionResult> AdicionarNivelEmTrilha(int intNivelId, int idTrilhaId)
        {
            ModelNivelTrilha? objModelNivel = new();
            ModelTrilha? objModelTrilha = new();

            try
            {
                objModelNivel = await _context.NiveisTrilha.FindAsync(intNivelId);

                if (objModelNivel == null)
                {
                    return NotFound(new { message = "Nível não encontrado" });
                }

                objModelTrilha = await _context.Trilhas.FindAsync(idTrilhaId);
                if (objModelTrilha == null)
                {
                    return NotFound(new { message = "Trilha não encontrada" });
                }

                objModelNivel.IdTrilha = idTrilhaId;
                objModelNivel.Trilha = objModelTrilha;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NivelExists(intNivelId))
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

        private bool NivelExists(int intId)
        {
            return _context.NiveisTrilha.Any(e => e.IdNivel == intId);
        }
    }
}
