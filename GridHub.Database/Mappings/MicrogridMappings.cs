using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GridHub.Database.Models;

namespace GridHub.Database.Mappings
{
    public class MicrogridMapping : IEntityTypeConfiguration<Microgrid>
    {
        public void Configure(EntityTypeBuilder<Microgrid> builder)
        {
            builder.ToTable("GRIDHUB_MICROGRIDS");

            builder.HasKey(m => m.MicrogridId);

            builder.Property(m => m.MicrogridId)
                   .ValueGeneratedOnAdd();

            builder.Property(m => m.UsuarioId)
                   .IsRequired();

            builder.Property(m => m.EspacoId)
                   .IsRequired();

            builder.Property(m => m.NomeMicrogrid)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.FotoMicrogrid)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(m => m.RadiacaoSolarNecessaria)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(m => m.TopografiaNecessaria)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.AreaTotalNecessaria)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(m => m.VelocidadeVentoNecessaria)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(m => m.FonteEnergia)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(m => m.MetaFinanciamento)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.HasOne<Usuario>()
                   .WithMany()
                   .HasForeignKey(m => m.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Espaco>()
                   .WithMany()
                   .HasForeignKey(m => m.EspacoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
