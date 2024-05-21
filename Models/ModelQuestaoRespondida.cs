using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BitBeakAPI.Models
{
    public class ModelQuestaoRespondida
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        public ModelUsuario Usuario { get; set; } = new();

        [Required]
        [ForeignKey("Questao")]
        public int IdQuestao { get; set; }

        public ModelQuestao Questao { get; set; } = new();
    }

}
