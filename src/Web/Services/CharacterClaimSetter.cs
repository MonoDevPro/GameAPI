using GameWeb.Application.Common.Interfaces;
using System.Security.Claims;

namespace GameWeb.Web.Services;

public class CharacterClaimSetter : ICharacterClaimSetter, ICurrentCharacterAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CharacterClaimSetter(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? CurrentCharacterId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirst("character_id")?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public void EnsureCharacterClaim(int characterId)
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal is null) return;
        var identity = principal.Identity as ClaimsIdentity;
        if (identity is null) return;
        if (!identity.HasClaim(c => c.Type == "character_id"))
        {
            identity.AddClaim(new Claim("character_id", characterId.ToString()));
        }
    }
}
