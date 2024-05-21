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
    public class NiveisUsuarioController : ControllerBase
    {
        private readonly BitBeakContext _context;

        public NiveisUsuarioController(BitBeakContext context)
        {
            _context = context;
        }

        // GET: api/NiveisUsuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelNivelUsuario>>> GetNiveisUsuario()
        {
            return await _context.NiveisUsuario.ToListAsync();
        }

        // GET: api/NiveisUsuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModelNivelUsuario>> GetNivelUsuario(int id)
        {
            var nivelUsuario = await _context.NiveisUsuario.FindAsync(id);

            if (nivelUsuario == null)
            {
                return NotFound();
            }

            return nivelUsuario;
        }

        // POST: api/NiveisUsuario
        [HttpPost]
        public async Task<ActionResult<ModelNivelUsuario>> PostNivelUsuario(ModelNivelUsuario nivelUsuario)
        {
            _context.NiveisUsuario.Add(nivelUsuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNivelUsuario), new { id = nivelUsuario.IdNivelUsuario }, nivelUsuario);
        }

        // PUT: api/NiveisUsuario/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNivelUsuario(int id, ModelNivelUsuario nivelUsuario)
        {
            if (id != nivelUsuario.IdNivelUsuario)
            {
                return BadRequest();
            }

            _context.Entry(nivelUsuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NivelUsuarioExists(id))
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

        // DELETE: api/NiveisUsuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNivelUsuario(int id)
        {
            var nivelUsuario = await _context.NiveisUsuario.FindAsync(id);
            if (nivelUsuario == null)
            {
                return NotFound();
            }

            _context.NiveisUsuario.Remove(nivelUsuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NivelUsuarioExists(int id)
        {
            return _context.NiveisUsuario.Any(e => e.IdNivelUsuario == id);
        }
    }
}
