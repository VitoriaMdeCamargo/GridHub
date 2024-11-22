using GridHub.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace GridHub.Database.Mappings
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("GRIDHUB_USUARIOS");

            builder.HasKey(x => x.UsuarioId);

            builder.Property(x => x.UsuarioId)
                .HasColumnName("UsuarioId")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Email)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Senha)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Telefone)
                .HasMaxLength(20);

            builder.Property(x => x.FotoPerfil)
                .HasMaxLength(255);

            builder.Property(x => x.DataCriacao)
                .HasColumnType("Date")
                .IsRequired();

            builder.HasIndex(x => x.Email)
                .IsUnique();
        }
    }
}
