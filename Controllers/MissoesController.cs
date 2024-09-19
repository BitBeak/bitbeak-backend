using BitBeakAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Função para obter missões ativas do usuario
        /// </summary>
        /// <param name="intIdUsuario"></param>
        /// <returns></returns>
        [HttpGet("ObterMissoesAtivasUsuario/{intIdUsuario}")]
        public async Task<ActionResult> ObterMissoesAtivasUsuario(int intIdUsuario)
        {
            try
            {
                var objUsuario = await _context.Usuarios.FindAsync(intIdUsuario);
                if (objUsuario == null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                var objMissoesAtivas = await _context.ProgressoMissoes
                    .Where(pm => pm.IdUsuario == intIdUsuario && !pm.Completa)
                    .Include(pm => pm.Missao)
                    .Select(pm => new {
                        pm.Missao.Descricao,
                        pm.ProgressoAtual,
                        pm.Missao.Objetivo,
                        pm.Completa
                    })
                    .ToListAsync();

                if (!objMissoesAtivas.Any())
                {
                    return NotFound("Nenhuma missão ativa encontrada para este usuário.");
                }

                return Ok(objMissoesAtivas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
