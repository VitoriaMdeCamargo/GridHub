using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GridHub.Database.Models;

namespace GridHub.Database.Mappings
{
    public class RelatorioMapping : IEntityTypeConfiguration<Relatorio>
    {
        public void Configure(EntityTypeBuilder<Relatorio> builder)
        {
            builder.ToTable("GRIDHUB_RELATORIOS");

            builder.HasKey(r => r.RelatorioId);

            builder.Property(r => r.RelatorioId)
                   .ValueGeneratedOnAdd();

            builder.Property(r => r.MicrogridId)
                   .IsRequired();

            builder.Property(r => r.EnergiaGerada)
                   .HasPrecision(18, 2)
                   .IsRequired()
                   .HasDefaultValue(0.0);

            builder.Property(r => r.TempPainelSolar)
                   .HasPrecision(18, 2)
                   .IsRequired()
                   .HasDefaultValue(25.0);

            builder.Property(r => r.LucroGerado)
                   .HasPrecision(18, 2)
                   .IsRequired()
                   .HasDefaultValue(0.0);

            builder.HasOne<Microgrid>()
                   .WithMany()
                   .HasForeignKey(r => r.MicrogridId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
