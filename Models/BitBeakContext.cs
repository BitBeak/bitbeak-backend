using Microsoft.EntityFrameworkCore;

namespace BitBeakAPI.Models
{
    public class BitBeakContext : DbContext
    {
        public BitBeakContext(DbContextOptions<BitBeakContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
