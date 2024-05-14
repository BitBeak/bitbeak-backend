﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitBeakAPI.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string? SenhaCriptografada { get; set; }  // Senha criptografada

        [NotMapped]
        public string Senha { get; set; } // Senha em texto simples para entrada

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
    }
}
