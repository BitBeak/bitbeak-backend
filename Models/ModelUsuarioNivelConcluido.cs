using System;
using System.ComponentModel.DataAnnotations;

namespace BitBeakAPI.Models
{
    public class ModelUsuarioNivelConcluido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int IdTrilha { get; set; }

        [Required]
        public int IdNivel { get; set; }

        public DateTime DataConclusao { get; set; } = DateTime.Now;  // Data da conclusão
    }
}
