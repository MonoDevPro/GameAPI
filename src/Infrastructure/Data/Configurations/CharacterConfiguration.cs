using GameWeb.Domain.Entities;
using GameWeb.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameWeb.Infrastructure.Data.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.HasQueryFilter(c => c.IsActive);
        
        // Configura propriedades, como o tamanho máximo do nome
        builder.Property(c => c.Name)
            .HasConversion(v => v.Value, v => CharacterName.Create(v))
            .HasMaxLength(20)
            .IsRequired();
        
        // Garante que o nome seja único
        builder.HasIndex(c => c.Id, "IX_Character_Id").IsUnique();
        builder.HasIndex(c => c.Name, "IX_Character_Name").IsUnique();
        
        // Configura propriedades complexas como Value Objects
        // Configura Position como um componente "owned"
        builder.OwnsOne(c => c.Position, positionBuilder =>
        {
            // Mapeia as propriedades de Vector2D para colunas específicas
            positionBuilder.Property(p => p.X).HasColumnName("PositionX");
            positionBuilder.Property(p => p.Y).HasColumnName("PositionY");
        });

        // Configura Direction da mesma forma
        builder.OwnsOne(c => c.Direction, directionBuilder =>
        {
            directionBuilder.Property(p => p.X).HasColumnName("DirectionX");
            directionBuilder.Property(p => p.Y).HasColumnName("DirectionY");
        });
    }
}
