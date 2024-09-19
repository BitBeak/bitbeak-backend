using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BitBeakAPI.Models
{
    public class ModelMissaoProgresso
    {
        [Key]
        public int IdMissaoAtiva { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public ModelUsuario Usuario { get; set; }

        [ForeignKey("Missao")]
        public int IdMissao { get; set; }
        public ModelMissao Missao { get; set; }

        public int ProgressoAtual { get; set; } = 0;

        public bool Completa { get; set; } = false;
    }

}
