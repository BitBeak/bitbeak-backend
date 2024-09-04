using BitBeakAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class ModelAmizade
{
    [Key]
    public int IdAmizade { get; set; }

    [Required]
    public int IdUsuario { get; set; }

    [ForeignKey("IdUsuario")]
    public ModelUsuario Usuario { get; set; }

    [Required]
    public int IdAmigo { get; set; }

    [ForeignKey("IdAmigo")]
    public ModelUsuario Amigo { get; set; }
}
