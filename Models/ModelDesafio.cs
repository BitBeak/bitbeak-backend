using BitBeakAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class ModelDesafio
{
    [Key]
    public int IdDesafio { get; set; }

    [Required]
    public int IdDesafiante { get; set; }

    [ForeignKey("IdDesafiante")]
    public ModelUsuario? Desafiante { get; set; }

    [Required]
    public int IdDesafiado { get; set; }

    [ForeignKey("IdDesafiado")]
    public ModelUsuario? Desafiado { get; set; }

    [Required]
    public int IdTrilha { get; set; }

    [ForeignKey("IdTrilha")]
    public ModelTrilha? Trilha { get; set; }

    public bool DesafianteJogando { get; set; } = true;

    public int InsigniasDesafiante { get; set; } = 0; 

    public int InsigniasDesafiado { get; set; } = 0;

    public int NivelDesafiante { get; set; }

    public int NivelDesafiado { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public DateTime UltimaAtualizacao { get; set; } = DateTime.Now;

    public bool Finalizado { get; set; } = false;

    public bool StatusAceiteDesafio { get; set; } = true;

}
