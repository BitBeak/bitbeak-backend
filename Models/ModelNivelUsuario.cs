using System.ComponentModel.DataAnnotations;

namespace BitBeakAPI.Models
{
    public class ModelNivelUsuario
    {
        [Key]
        public int IdNivelUsuario { get; set; }

        [Required]
        public int NivelUsuario { get; set; }

        [Required]
        public int ExperienciaNecessaria { get; set; }
    }
}
