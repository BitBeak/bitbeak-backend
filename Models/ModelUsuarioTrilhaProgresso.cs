using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitBeakAPI.Models
{
    public class ModelUsuarioTrilhaProgresso
    {
        [Key]
        public int IdProgresso { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        public ModelUsuario Usuario { get; set; } = new();

        [Required]
        [ForeignKey("Trilha")]
        public int IdTrilha { get; set; }

        public ModelTrilha Trilha { get; set; } = new();

        public int Nivel { get; set; }

        public int Experiencia { get; set; }
    }
}
