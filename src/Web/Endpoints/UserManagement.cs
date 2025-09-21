using GameWeb.Application.Characters.Queries.Admin;
using GameWeb.Domain.Constants;

namespace GameWeb.Web.Endpoints;

public class UserManagement : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        // Define um prefixo e segurança para TODO o grupo administrativo
        group.RequireAuthorization(Policies.CanManageUsers); // Opcional, pode ser na camada do CQRS

        // Cria um subgrupo para manter as rotas de personagens organizadas
        var charactersGroup = group.MapGroup("/characters");
        charactersGroup.MapGet("/all", GetAllCharacters)
            .WithSummary("Get a paginated list of all characters (Admin only).");
        
        // Futuros endpoints administrativos (ex: banir utilizador, ver logs) podem ser adicionados aqui.
    }

    // --- MÉTODOS DE HANDLER PARA CADA ENDPOINT ---

    /// <summary>
    /// Obtém uma lista paginada de todos os personagens (ativos e inativos).
    /// </summary>
    public async Task<IResult> GetAllCharacters(ISender sender, [AsParameters] GetAllCharactersQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }
}
