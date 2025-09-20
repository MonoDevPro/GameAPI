namespace GameWeb.Application.Common.Interfaces;

/// <summary>
/// Fornece acesso somente leitura ao personagem selecionado (Id) do usuário corrente se já resolvido em camada superior.
/// </summary>
public interface ICurrentCharacterAccessor
{
    int? CurrentCharacterId { get; }
}
