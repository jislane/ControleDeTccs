using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaDeControleDeTCCs.Models;

namespace SistemaDeControleDeTCCs.Data
{
    public class SistemaDeControleDeTCCsContext : IdentityDbContext<Usuario>
    {
        public SistemaDeControleDeTCCsContext(DbContextOptions<SistemaDeControleDeTCCsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Banca>().HasOne(x => x.TipoUsuario).WithMany().HasForeignKey(x => x.TipoUsuarioId).IsRequired().OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Banca>().HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).IsRequired().OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Usuario>().HasOne(x => x.TipoUsuario).WithMany().HasForeignKey(x => x.TipoUsuarioId).IsRequired().OnDelete(DeleteBehavior.ClientCascade);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Tcc> Tccs { get; set; }
        public DbSet<Banca> Banca { get; set; }
        public DbSet<Calendario> Calendario { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<TipoUsuario> TipoUsuario { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<FileTCC> FileTCC { get; set; }
        public DbSet<LogAuditoria> LogAuditoria { get; set; }
    }
}
