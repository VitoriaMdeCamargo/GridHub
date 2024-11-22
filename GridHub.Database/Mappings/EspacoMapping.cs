using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GridHub.Database.Models;

namespace GridHub.Database.Mappings
{
    public class EspacoMapping : IEntityTypeConfiguration<Espaco>
    {
        public void Configure(EntityTypeBuilder<Espaco> builder)
        {
            builder.ToTable("GRIDHUB_ESPACOS");

            builder.HasKey(e => e.EspacoId);

            builder.Property(e => e.EspacoId)
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.UsuarioId)
                   .IsRequired();

            builder.Property(e => e.Endereco)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.NomeEspaco)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.FotoEspaco)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(e => e.FonteEnergia)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(e => e.OrientacaoSolar)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(e => e.MediaSolar)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(e => e.Topografia)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.AreaTotal)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(e => e.DirecaoVento)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(e => e.VelocidadeVento)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.HasOne<Usuario>()  
                   .WithMany()  
                   .HasForeignKey(e => e.UsuarioId)  
                   .OnDelete(DeleteBehavior.Cascade);  
        }
    }
}
