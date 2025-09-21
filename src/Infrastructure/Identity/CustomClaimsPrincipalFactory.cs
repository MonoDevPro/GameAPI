using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GameWeb.Infrastructure.Identity;

/// <summary>
/// Customiza a criação do ClaimsPrincipal para adicionar claims persistentes / customizados.
/// </summary>
public class CustomClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, optionsAccessor)
{
    
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        if (user.ActiveCharacterId.HasValue)
        {
            identity.AddClaim(new Claim("character_id", user.ActiveCharacterId.Value.ToString()));
        }
        return identity;
    }
}
