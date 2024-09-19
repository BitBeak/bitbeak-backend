using System.ComponentModel.DataAnnotations;

namespace BitBeakAPI.Models
{
    public enum TipoMissao
    {
        Questao,
        Nivel,
        Trilha,
        Desafio
    }
    public class ModelMissao
    {
        [Key]
        public int IdMissao { get; set; }

        [Required]
        public string? Nome { get; set; }

        [Required]
        public string? Descricao { get; set; }

        [Required]
        public TipoMissao TipoMissao { get; set; } 

        [Required]
        public int Objetivo { get; set; } 

        [Required]
        public int RecompensaPenas { get; set; }

        [Required]
        public int RecompensaExperiencia { get; set; }

        public bool Inicial { get; set; } = false; 
    }

}
