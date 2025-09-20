using GameWeb.Domain.Entities;
using GameWeb.Domain.Enums;
using GameWeb.Domain.ValueObjects;

namespace GameWeb.Application.Common.Models;

public class CharacterLookup
{
    // --- Dados Frios / de Identificação ---
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Gender Gender { get; init; }
    public Vocation Vocation { get; init; }
    
    public CharacterStats Stats { get; init; } = null!;
    public Vector2D Position { get; init; } = null!;
    public Vector2D Direction { get; init; } = null!;
    
    // Propriedades que precisam ser públicas para o EF Core mapear a chave estrangeira.
    public string OwnerId { get; init; } = null!;
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Character, CharacterLookup>();
        }
    }
}
