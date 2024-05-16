using Microsoft.EntityFrameworkCore;

namespace BitBeakAPI.Models
{
    public class BitBeakContext : DbContext
    {
        public BitBeakContext(DbContextOptions<BitBeakContext> options) : base(options)
        {
        }

        public DbSet<ModelUsuario> Usuarios { get; set; }
        public DbSet<ModelTrilha> Trilhas { get; set; }
        public DbSet<ModelQuestao> Questoes { get; set; }
        public DbSet<ModelNivelTrilha> NiveisTrilha { get; set; }
        public DbSet<ModelUsuarioTrilhaProgresso> UsuariosTrilhasProgresso { get; set; }
        public DbSet<OpcaoResposta> OpcoesRespostas { get; set; }
        public DbSet<Lacuna> Lacunas { get; set; }
    }
}
