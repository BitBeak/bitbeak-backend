using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitBeakAPI.Models
{
    public class ModelUsuario
    {
        #region Usuario

        [Key] public int IdUsuario { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SenhaCriptografada { get; set; }  // Senha criptografada

        [NotMapped]
        public string Senha { get; set; } = string.Empty; // Senha em texto simples para entrada

        #endregion

        #region ResetSenha

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        #endregion

        #region Infos

        public int Nivel { get; set; }
        public int Experiencia { get; set; }
        public int Penas { get; set; }

        public ICollection<ModelUsuarioTrilhaProgresso> TrilhasProgresso { get; set; } = new List<ModelUsuarioTrilhaProgresso>();

        #endregion
    }
}
