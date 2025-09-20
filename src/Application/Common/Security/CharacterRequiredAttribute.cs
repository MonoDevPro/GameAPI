using GameWeb.Domain.Enums;

namespace GameWeb.Application.Common.Security;

/// <summary>
/// Atributo base para todos os requisitos de validação de personagem.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public abstract class CharacterRequiredAttribute : Attribute
{
    public bool AllowNotOwner { get; set; } = false;
}
