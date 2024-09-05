using BitBeakAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ModelHistoricoDesafio
{
    [Key]
    public int IdHistoricoConfronto { get; set; }

    [Required]
    public int IdDesafio { get; set; }

    [Required]
    public int IdDesafiante { get; set; }

    [Required]
    public int IdDesafiado { get; set; }

    [Required]
    public int IdVencedor { get; set; }

    public DateTime DataConfronto { get; set; } = DateTime.Now;

    public ModelUsuario? Desafiante { get; set; } 
    public ModelUsuario? Desafiado { get; set; } 
}
