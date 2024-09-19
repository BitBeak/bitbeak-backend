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
            public DbSet<ModelNivelUsuario> NiveisUsuario { get; set; }
            public DbSet<ModelUsuarioTrilhaConcluida> UsuarioTrilhasConcluidas { get; set; }
            public DbSet<ModelUsuarioNivelConcluido> UsuarioNiveisConcluidos { get; set; }
            public DbSet<ModelAmizade> Amizades { get; set; }       
            public DbSet<ModelDesafio> Desafios { get; set; }  
            public DbSet<ModelHistoricoDesafio> HistoricoDesafios { get; set; }
            public DbSet<ModelMissao> Missoes { get; set; }
            
            public DbSet<ModelMissaoProgresso> ProgressoMissoes { get; set; }

            public BitBeakContext(DbContextOptions<BitBeakContext> options) : base(options)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlServer("Server=tcp:bitbeak.database.windows.net,1433;Initial Catalog=BitBeak;Persist Security Info=False;User ID=BitBeak;Password=Tcc@2024;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5, 
                            maxRetryDelay: TimeSpan.FromSeconds(30), 
                            errorNumbersToAdd: null);
                    });
                }
            }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<ModelTrilha>()
                    .HasMany(t => t.Niveis);

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

                modelBuilder.Entity<ModelAmizade>()
                        .HasOne(a => a.Usuario)
                        .WithMany(u => u.Amigos)
                        .HasForeignKey(a => a.IdUsuario)
                        .OnDelete(DeleteBehavior.Restrict); 

                modelBuilder.Entity<ModelAmizade>()
                    .HasOne(a => a.Amigo)
                    .WithMany()
                    .HasForeignKey(a => a.IdAmigo)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<ModelDesafio>()
                    .HasOne(d => d.Desafiante)
                    .WithMany()
                    .HasForeignKey(d => d.IdDesafiante)
                    .OnDelete(DeleteBehavior.NoAction);

                modelBuilder.Entity<ModelDesafio>()
                    .HasOne(d => d.Desafiado)
                    .WithMany()
                    .HasForeignKey(d => d.IdDesafiado)
                    .OnDelete(DeleteBehavior.NoAction);

                modelBuilder.Entity<ModelHistoricoDesafio>()
                   .HasOne(h => h.Desafiante)
                   .WithMany()
                   .HasForeignKey(h => h.IdDesafiante)
                   .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<ModelHistoricoDesafio>()
                    .HasOne(h => h.Desafiado)
                    .WithMany()
                    .HasForeignKey(h => h.IdDesafiado)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
