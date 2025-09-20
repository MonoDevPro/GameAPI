using GameWeb.Infrastructure.Identity;

namespace GameWeb.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder app)
    {
        app.MapIdentityApi<ApplicationUser>();
    }
}
