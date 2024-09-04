using Microsoft.EntityFrameworkCore;
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

        public string CodigoDeAmizade { get; set; } = string.Empty;

        public ICollection<ModelAmizade> Amigos { get; set; } = new List<ModelAmizade>();
        public string? SenhaCriptografada { get; set; }  

        [NotMapped]
        public string Senha { get; set; } = string.Empty; 

        #endregion

        #region ResetSenha

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        #endregion

        #region Infos

        [Required]
        public int NivelUsuario { get; set; }

        [Required]
        public int ExperienciaUsuario { get; set; }

        public int ExperienciaQuinzenalUsuario { get; set; }

        [Required]
        public int Penas { get; set; }

        #endregion

    }
}
