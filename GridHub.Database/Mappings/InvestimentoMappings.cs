using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GridHub.Database.Models;

namespace GridHub.Database.Mappings
{
    public class InvestimentoMapping : IEntityTypeConfiguration<Investimento>
    {
        public void Configure(EntityTypeBuilder<Investimento> builder)
        {
            builder.ToTable("GRIDHUB_INVESTIMENTOS");

            builder.HasKey(i => i.InvestimentoId);

            builder.Property(i => i.InvestimentoId)
                   .ValueGeneratedOnAdd();

            builder.Property(i => i.UsuarioId)
                   .IsRequired();

            builder.Property(i => i.MicrogridId)
                   .IsRequired();

            builder.Property(i => i.DescricaoProposta)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne<Usuario>()
                   .WithMany()
                   .HasForeignKey(i => i.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Microgrid>()
                   .WithMany()
                   .HasForeignKey(i => i.MicrogridId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
