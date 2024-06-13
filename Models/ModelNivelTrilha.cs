using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BitBeakAPI.Models
{
    public class ModelNivelTrilha
    {
        [Key]
        public int IdNivel { get; set; }

        [Required]
        public int Nivel { get; set; }

        public string NivelName { get; set; }   = string.Empty;

        [Required]
        [ForeignKey("Trilha")]
        public int IdTrilha { get; set; }

        [JsonIgnore]
        public ModelTrilha Trilha { get; set; } = new();

        public ICollection<ModelQuestao> Questoes { get; set; } = new List<ModelQuestao>();
    }
}
