namespace GameWeb.Application.Common.Interfaces;

/// <summary>
/// Permite que a Application solicite a inclusão de uma claim de Character sem conhecer HttpContext/Identity.
/// Implementação concreta vive na camada Web.
/// </summary>
public interface ICharacterClaimSetter
{
    void EnsureCharacterClaim(int characterId);
}
