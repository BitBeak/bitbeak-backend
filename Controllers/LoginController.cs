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

            // Autenticação bem-sucedida
            // Aqui, você pode retornar um token de autenticação ou uma resposta de sucesso
            return Ok(new { Message = "Login bem-sucedido" });
        }
    }
}
