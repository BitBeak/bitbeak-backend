using BitBeakAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissoesController : ControllerBase
    {
        private readonly BitBeakContext _context;

        public MissoesController(BitBeakContext context)
        {
            _context = context;
        }

        [HttpPost("AdicionarMissao")]
        public async Task<IActionResult> AdicionarMissao(ModelMissao objNovaMissao)
        {
            try
            {
                _context.Missoes.Add(objNovaMissao);
                await _context.SaveChangesAsync();
                return Ok("Missão adicionada com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao adicionar missão: " + ex.Message);
            }
        }
    }
}
