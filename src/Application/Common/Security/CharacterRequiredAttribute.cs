using GameWeb.Domain.Enums;

namespace GameWeb.Application.Common.Security;

/// <summary>
/// Indica que a execução do request exige um personagem atualmente selecionado.
/// Caso <see cref="AllowNotOwner"/> seja true, permite que o personagem validado não pertença ao usuário (cenário PvP / espectador futuro).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class CharacterRequiredAttribute : Attribute
{
    public bool AllowNotOwner { get; init; } = false;
}
