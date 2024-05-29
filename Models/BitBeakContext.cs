    using Microsoft.EntityFrameworkCore;

    namespace BitBeakAPI.Models
    {
        public class BitBeakContext : DbContext
        {
            public DbSet<ModelUsuario> Usuarios { get; set; }
            public DbSet<ModelTrilha> Trilhas { get; set; }
            public DbSet<ModelNivelTrilha> NiveisTrilha { get; set; }
            public DbSet<ModelQuestao> Questoes { get; set; }
            public DbSet<OpcaoResposta> OpcoesResposta { get; set; }
            public DbSet<Lacuna> Lacunas { get; set; }
            public DbSet<CasoTeste> CasoTeste { get; set; }
            public DbSet<ModelNivelUsuario> NiveisUsuario { get; set; }
            public DbSet<ModelUsuarioTrilhaProgresso> UsuarioTrilhaProgresso { get; set; }
            public DbSet<ModelQuestaoRespondida> QuestoesRespondidas { get; set; }

        public BitBeakContext(DbContextOptions<BitBeakContext> options) : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<ModelTrilha>()
                    .HasMany(t => t.Niveis)
                    .WithOne(n => n.Trilha)
                    .HasForeignKey(n => n.IdTrilha);

                modelBuilder.Entity<ModelNivelTrilha>()
                    .HasMany(n => n.Questoes)
                    .WithOne(q => q.Nivel)
                    .HasForeignKey(q => q.IdNivel);

                modelBuilder.Entity<ModelQuestao>()
                    .HasMany(q => q.Opcoes)
                    .WithOne(o => o.Questao)
                    .HasForeignKey(o => o.IdQuestao);

                modelBuilder.Entity<ModelQuestao>()
                    .HasMany(q => q.Lacunas)
                    .WithOne(l => l.Questao)
                    .HasForeignKey(l => l.IdQuestao);
            }
        }
    }
