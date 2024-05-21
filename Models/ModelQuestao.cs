using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BitBeakAPI.Models
{
    public enum TipoQuestao
    {
        Pergunta,
        Lacuna,
        Codificacao
    }

    public class ModelQuestao
    {
        [Key]
        public int IdQuestao { get; set; }

        [Required]
        [StringLength(1000)]
        public string Enunciado { get; set; } = string.Empty;

        [Required]
        public TipoQuestao Tipo { get; set; }

        [ForeignKey("NivelTrilha")]
        public int? IdNivel { get; set; }

        [JsonIgnore]
        public ModelNivelTrilha? Nivel { get; set; }

        public ICollection<OpcaoResposta> Opcoes { get; set; } = new List<OpcaoResposta>();

        public ICollection<Lacuna> Lacunas { get; set; } = new List<Lacuna>();

        public string SolucaoEsperada { get; set; } = string.Empty;
    }

    public class OpcaoResposta
    {
        [Key]
        public int IdOpcao { get; set; }

        [Required]
        public string Texto { get; set; }

        [Required]
        public bool Correta { get; set; }

        [ForeignKey("Questao")]
        public int IdQuestao { get; set; }

        [JsonIgnore]
        public ModelQuestao Questao { get; set; }
    }

    public class Lacuna
    {
        [Key]
        public int IdLacuna { get; set; }

        [Required]
        public string? ColunaA { get; set; }

        [Required]
        public string? ColunaB { get; set; }

        [ForeignKey("Questao")]
        public int IdQuestao { get; set; }

        [JsonIgnore] 
        public ModelQuestao? Questao { get; set; }
    }
}
