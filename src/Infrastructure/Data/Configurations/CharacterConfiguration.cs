using GameWeb.Domain.Entities;
using GameWeb.Domain.ValueObjects;
using GameWeb.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameWeb.Infrastructure.Data.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        // Configura propriedades, como o tamanho máximo do nome
        builder.Property(c => c.Name)
            .HasMaxLength(50)
            .IsRequired();

        // Configura o relacionamento de posse
        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.Characters)
            .HasForeignKey(c => c.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(c => c.Name).IsUnique();
        
        // Configura propriedades complexas como Value Objects
        builder.Property(c => c.Position)
            .HasConversion(
                v => v.ToString(), // Converte para string para armazenar no banco
                v => Vector2D.FromString(v)) // Converte de volta para Vector2D ao ler do banco
            .IsRequired();
        builder.Property(c => c.Direction)
            .HasConversion(
                v => v.ToString(),
                v => Vector2D.FromString(v))
            .IsRequired();
    }
}
