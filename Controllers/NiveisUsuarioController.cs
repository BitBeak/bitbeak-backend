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
        [HttpGet("ObterListaNivelUsuario")]
        public async Task<ActionResult<IEnumerable<ModelNivelUsuario>>> ObterListaNivelUsuario()
        {
            return await _context.NiveisUsuario.ToListAsync();
        }

        // GET: api/NiveisUsuario/5
        [HttpGet("ListarDadosNivelUsuario/{intIdNivelUsuario}")]
        public async Task<ActionResult<ModelNivelUsuario>> ListarDadosNivelUsuario(int intIdNivelUsuario)
        {
            var objNivelUsuario = await _context.NiveisUsuario.FindAsync(intIdNivelUsuario);

            if (objNivelUsuario == null)
            {
                return NotFound();
            }

            return objNivelUsuario;
        }

        // POST: api/NiveisUsuario
        [HttpPost("CriarNivelUsuario")]
        public async Task<ActionResult<ModelNivelUsuario>> CriarNivelUsuario(ModelNivelUsuario objNivelUsuario)
        {
            _context.NiveisUsuario.Add(objNivelUsuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ListarDadosNivelUsuario), new { intIdNivelUsuario = objNivelUsuario.IdNivelUsuario }, objNivelUsuario);
        }

        // PUT: api/NiveisUsuario/5
        [HttpPut("EditarNivelUsuario/{intIdNivelUsuario}")]
        public async Task<IActionResult> EditarNivelUsuario(int intIdNivelUsuario, ModelNivelUsuario objNivelUsuario)
        {
            if (intIdNivelUsuario != objNivelUsuario.IdNivelUsuario)
            {
                return BadRequest();
            }

            _context.Entry(objNivelUsuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NivelUsuarioExists(intIdNivelUsuario))
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
        [HttpDelete("ExcluirNivelUsuario/{intIdNivelUsuario}")]
        public async Task<IActionResult> ExcluirNivelUsuario(int intIdNivelUsuario)
        {
            var objNivelUsuario = await _context.NiveisUsuario.FindAsync(intIdNivelUsuario);
            if (objNivelUsuario == null)
            {
                return NotFound();
            }

            _context.NiveisUsuario.Remove(objNivelUsuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NivelUsuarioExists(int intIdNivelUsuario)
        {
            return _context.NiveisUsuario.Any(e => e.IdNivelUsuario == intIdNivelUsuario);
        }
    }
}
