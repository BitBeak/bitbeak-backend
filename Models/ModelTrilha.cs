using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BitBeakAPI.Models
{
    public class ModelTrilha
    {
        [Key]
        public int IdTrilha { get; set; }

        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500)]
        public string Descricao { get; set; } = string.Empty;

        public ICollection<ModelNivelTrilha> Niveis { get; set; } = new List<ModelNivelTrilha>();
    }
}
