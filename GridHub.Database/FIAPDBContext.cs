using GridHub.Database.Mappings;
using GridHub.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GridHub.Database
{
    public class FIAPDBContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Espaco> Espacos { get; set; }
        public DbSet<Microgrid> Microgrids{ get; set; }
        public DbSet<Investimento> Investimento { get; set; }
        public DbSet<Relatorio> Relatorio { get; set; }

        public FIAPDBContext(DbContextOptions<FIAPDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioMapping());
            modelBuilder.ApplyConfiguration(new EspacoMapping());

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Geração de ID para novos usuários
            foreach (var entry in ChangeTracker.Entries<Usuario>()
                .Where(e => e.State == EntityState.Added))
            {
                if (entry.Entity.UsuarioId == 0)
                {
                    var maxId = await Usuarios.AsNoTracking()
                        .OrderByDescending(u => u.UsuarioId)
                        .Select(u => u.UsuarioId)
                        .FirstOrDefaultAsync(cancellationToken);

                    entry.Entity.UsuarioId = maxId + 1;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
