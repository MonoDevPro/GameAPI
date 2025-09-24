using System.Reflection;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;
using GameWeb.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameWeb.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configura o relacionamento de posse
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Characters)
            .WithOne()
            .HasForeignKey(c => c.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.ActiveCharacter) // Um utilizador tem um personagem ativo (opcional)
            .WithMany() // Um personagem não precisa de ter uma lista de utilizadores que o têm como ativo
            .HasForeignKey(u => u.ActiveCharacterId) // A chave estrangeira está em ApplicationUser
            .IsRequired(false) // A FK é opcional (pode ser nula)
            .OnDelete(DeleteBehavior.SetNull); // A magia acontece aqui!
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
