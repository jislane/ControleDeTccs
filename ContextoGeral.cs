using Microsoft.EntityFrameworkCore;

namespace SistemaDeControleDeTCCs.Models
{
    public class ContextoGeral: DbContext
    {
        public ContextoGeral()
        {

        }
        public ContextoGeral(DbContextOptions<ContextoGeral> options)
            : base(options)
        {
        }

        public DbSet<Tcc> Tccs { get; set; }
        public DbSet<Banca> Banca { get; set; }
        public DbSet<Calendario> Calendario { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<TipoUsuario> TipoUsuario { get; set; }
        public DbSet<Usuario> Usuario { get; set; }

    }
}