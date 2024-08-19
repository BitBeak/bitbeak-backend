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
        public async Task<bool> Login([FromBody] ModelLogin objModelLogin)
        {
            try
            {
                var objUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == objModelLogin.Email);

                if (objUsuario == null)
                {
                    // Usuário não encontrado
                    return false;
                }

                // Descriptografar a senha armazenada
                string strSenhaDescriptografada = Security.Descriptografar(strTextoCriptografado: objUsuario.SenhaCriptografada!);

                // Verificar se a senha fornecida corresponde à senha armazenada
                if (objModelLogin.Senha != strSenhaDescriptografada)
                {
                    // Senha incorreta
                    return false;
                }

                // Autenticação bem-sucedida
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
