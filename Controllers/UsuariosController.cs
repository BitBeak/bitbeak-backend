using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BitBeakAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly BitBeakContext _context;
        private readonly EmailService _emailService;

        public UsuariosController(BitBeakContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        #region Funções de GET
        // GET: api/Usuarios
        /// <summary>
        /// Função para procurar usuários
        /// </summary>
        /// <returns>Retorna todos os usuários e seus dados</returns>
        [HttpGet("ObterListaUsuarios")]
        public async Task<ActionResult<IEnumerable<ModelUsuario>>> ObterListaUsuarios()
        {
            var objUsuarios = await _context.Usuarios.ToListAsync();

            // Por motivos de segurança, não retorne a senha criptografada
            foreach (var objUsuario in objUsuarios)
            {
                objUsuario.SenhaCriptografada = null;
            }

            return objUsuarios;
        }

        // GET: api/Usuarios/5
        /// <summary>
        /// Função para procurar um usuário específico 
        /// </summary>
        /// <param name="intId">Obrigatorio - Id do Usuário</param>
        /// <returns>Retorna os dados do usuário pesquisado</returns>
        [AllowAnonymous]
        [HttpGet("ListarDadosUsuario/{intId}")]
        public async Task<ActionResult<ModelUsuario>> ListarDadosUsuario(int intId)
        {
            var objUsuario = await _context.Usuarios.FindAsync(intId);

            if (objUsuario == null)
            {
                return NotFound();
            }

            // Descriptografar a senha antes de retorná-la
            if (!string.IsNullOrEmpty(objUsuario.SenhaCriptografada))
            {
                try
                {
                    objUsuario.Senha = Security.Descriptografar(objUsuario.SenhaCriptografada);
                }
                catch (CryptographicException ex)
                {
                    // Log da exceção para fins de debug
                    Console.WriteLine($"Erro ao descriptografar a senha do usuário {objUsuario.IdUsuario}: {ex.Message}");
                    return StatusCode(500, "Erro ao descriptografar a senha");
                }
            }

            // Não retorne a senha criptografada por motivos de segurança
            objUsuario.SenhaCriptografada = null;

            return objUsuario;
        }

        #endregion

        #region Funções de POST e PUT

        // POST: api/Usuarios
        /// <summary>
        /// Função para Cadastrar Usuários
        /// </summary>
        /// <param name="objUsuario"></param>
        /// <returns></returns>
        [HttpPost("CadastrarUsuario")]
        public async Task<ActionResult<ModelUsuario>> CadastrarUsuario(ModelUsuario objUsuario)
        {
            // Verificar se o e-mail já existe
            var objUsuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == objUsuario.Email);

            if (objUsuarioExistente != null)
            {
                return Conflict("Já existe uma conta com este e-mail.");
            }

            // Criptografar a senha fornecida pelo usuário antes de salvar
            if (!string.IsNullOrEmpty(objUsuario.Senha))
            {
                objUsuario.SenhaCriptografada = Security.Criptografar(objUsuario.Senha);
            }

            objUsuario.NivelUsuario = 1;

            _context.Usuarios.Add(objUsuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ListarDadosUsuario), new { intId = objUsuario.IdUsuario }, objUsuario);
        }

        // PUT: api/Usuarios/5
        [HttpPut("EditarUsuario/{intId}")]
        public async Task<IActionResult> EditarUsuario(int intId, ModelUsuario objUsuario)
        {
            if (intId != objUsuario.IdUsuario)
            {
                return BadRequest();
            }

            // Verificar se o e-mail já existe para outro usuário
            var objUsuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == objUsuario.Email && u.IdUsuario != intId);
            if (objUsuarioExistente != null)
            {
                return Conflict("Já existe uma conta com este e-mail.");
            }

            // Criptografar a senha fornecida pelo usuário antes de atualizar
            if (!string.IsNullOrEmpty(objUsuario.Senha))
            {
                objUsuario.SenhaCriptografada = Security.Criptografar(objUsuario.Senha);
            }

            _context.Entry(objUsuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(intId))
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

        [HttpPost("ResetarSenha")]
        public async Task<IActionResult> ResetarSenha([FromBody] string strEmail)
        {
            var objUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == strEmail);
            if (objUsuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            // Gerar um token de recuperação de senha
            objUsuario.PasswordResetToken = Guid.NewGuid().ToString();
            objUsuario.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token válido por 1 hora

            await _context.SaveChangesAsync();

            // Enviar email com o token de recuperação de senha
            await _emailService.SendPasswordResetEmail(objUsuario.Email, objUsuario.PasswordResetToken);

            return Ok("Token de recuperação de senha enviado para o email");
        }

        [HttpPost("TokenResetarSenha")]
        public async Task<IActionResult> TokenResetarSenha([FromBody] ModelResetSenha objModelResetPassword)
        {
            var objUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.PasswordResetToken == objModelResetPassword.Token);
            if (objUsuario == null || objUsuario.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest("Token inválido ou expirado");
            }

            // Redefinir a senha
            objUsuario.SenhaCriptografada = Security.Criptografar(objModelResetPassword.NovaSenha);
            objUsuario.PasswordResetToken = null;
            objUsuario.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("Senha redefinida com sucesso");
        }

        #endregion

        #region Funções de DELETE
        /// <summary>
        /// Função usuário específico
        /// </summary>
        /// <param name="intId"></param>
        /// <returns></returns>
        // DELETE: api/Usuarios/5
        [HttpDelete("ExcluirUsuario/{intId}")]
        public async Task<IActionResult> ExcluirUsuario(int intId)
        {
            var objUsuario = await _context.Usuarios.FindAsync(intId);
            if (objUsuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(objUsuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método para verificar se um usuário existe
        private bool UsuarioExists(int intId)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == intId);
        }

        #endregion
    }
}
