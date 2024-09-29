using BitBeakAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmizadeController : ControllerBase

    {
        private readonly BitBeakContext _context;

        public AmizadeController(BitBeakContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Função para adicionar amizade
        /// </summary>
        /// <param name="intIUsuario"></param>
        /// <param name="strCodigoAmizadeAmigo"></param>
        /// <returns></returns>
        [HttpPost("AdicionarAmigo")]
        public async Task<ActionResult> AdicionarAmigo(int intIdUsuario, string strCodigoAmizadeAmigo)
        {
            var objUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == intIdUsuario);

            if (objUsuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var objAmigo = await _context.Usuarios.FirstOrDefaultAsync(u => u.CodigoDeAmizade == strCodigoAmizadeAmigo);

            if (objAmigo == null)
            {
                return NotFound("Amigo não encontrado.");
            }

            var objAmizadeExistenteUsuario = await _context.Amizades.FirstOrDefaultAsync(a => a.IdUsuario == intIdUsuario && a.IdAmigo == objAmigo.IdUsuario);

            var objAmizadeExistenteAmigo = await _context.Amizades.FirstOrDefaultAsync(a => a.IdUsuario == objAmigo.IdUsuario && a.IdAmigo == intIdUsuario);

            if (objAmizadeExistenteUsuario != null || objAmizadeExistenteAmigo != null)
            {
                return BadRequest("Amizade já existe.");
            }

            var amizadeUsuario = new ModelAmizade
            {
                IdUsuario = intIdUsuario,
                IdAmigo = objAmigo.IdUsuario
            };

            var amizadeAmigo = new ModelAmizade
            {
                IdUsuario = objAmigo.IdUsuario,
                IdAmigo = intIdUsuario
            };

            _context.Amizades.Add(amizadeUsuario);
            _context.Amizades.Add(amizadeAmigo);
            await _context.SaveChangesAsync();

            return Ok("Amigo adicionado com sucesso.");
        }

        [HttpGet("ListarAmigos/{intIdUsuario}")]
        public async Task<ActionResult<IEnumerable<object>>> ListarAmigos(int intIdUsuario)
        {
            var objUsuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == intIdUsuario);

            if (objUsuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var lstAmigos = await _context.Amizades
                .Where(a => a.IdUsuario == intIdUsuario)
                .Include(a => a.Amigo)
                .Select(a => new
                {
                    IdUsuario = a.Amigo.IdUsuario,
                    Nome = a.Amigo.Nome,
                    Email = a.Amigo.Email,
                    NivelUsuario = a.Amigo.NivelUsuario
                })
                .ToListAsync();

            if (lstAmigos == null || lstAmigos.Count == 0)
            {
                return NotFound("Nenhum amigo encontrado.");
            }

            return Ok(lstAmigos);
        }

        [HttpDelete("RemoverAmizade/{intIdUsuario}/{intIdAmigo}")]
        public async Task<ActionResult> RemoverAmizade(int intIdUsuario, int intIdAmigo)
        {
            var objAmizade = await _context.Amizades
                .FirstOrDefaultAsync(a => a.IdUsuario == intIdUsuario && a.IdAmigo == intIdAmigo);

            var objAmizadeReversa = await _context.Amizades
                .FirstOrDefaultAsync(a => a.IdUsuario == intIdAmigo && a.IdAmigo == intIdUsuario);

            if (objAmizade == null && objAmizadeReversa == null)
            {
                return NotFound("Amizade não encontrada.");
            }

            if (objAmizade != null)
            {
                _context.Amizades.Remove(objAmizade);
            }

            if (objAmizadeReversa != null)
            {
                _context.Amizades.Remove(objAmizadeReversa);
            }

            await _context.SaveChangesAsync();

            return Ok("Amizade removida com sucesso.");
        }

    }
}
