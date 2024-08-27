using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BitBeakAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity.Data;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly BitBeakContext _context;

        public LoginController(BitBeakContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] ModelLogin objModelLogin)
        {
            try
            {
                var objUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == objModelLogin.Email);

                if (objUsuario == null)
                {
                    return Unauthorized("Usuário não encontrado.");
                }

                // Descriptografar a senha armazenada
                string strSenhaDescriptografada = Security.Descriptografar(strTextoCriptografado: objUsuario.SenhaCriptografada!);

                // Verificar se a senha fornecida corresponde à senha armazenada
                if (objModelLogin.Senha != strSenhaDescriptografada)
                {
                    return Unauthorized("Senha incorreta.");
                }

                // Autenticação bem-sucedida, retornar o IdUsuario junto com a mensagem
                return Ok(new { IdUsuario = objUsuario.IdUsuario, Message = "Login bem-sucedido" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }
}
