using GameWeb.Application.Characters.Commands.CreateCharacter;
using GameWeb.Application.Characters.Commands.SelectCharacter;
using GameWeb.Application.Characters.Commands.DeleteCharacter;
using GameWeb.Application.Characters.Queries.GetMyCharacters;
using GameWeb.Application.Characters.Queries.GetCharacterById;

namespace GameWeb.Web.Endpoints;

public class Characters : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        // Agrupamos e aplicamos a autorização uma única vez
        group.MapGroup("characters")
                       .WithTags("Characters") // Adiciona uma tag para o Swagger/OpenAPI
                       .RequireAuthorization();

        // O método Map agora é apenas um "índice" de rotas, muito mais limpo
        group.MapPost("/", CreateCharacter);
        group.MapPost("/{id:int}/select", SelectCharacter);
        group.MapDelete("/{id:int}", DeleteCharacter);
        group.MapGet("/me", GetMyCharacters);
        group.MapGet("/{id:int}", GetCharacterById);
    }

    // --- MÉTODOS DE HANDLER PARA CADA ENDPOINT ---

    /// <summary>
    /// Cria um novo personagem para o usuário logado.
    /// </summary>
    private static async Task<IResult> CreateCharacter(ISender sender, CreateCharacterCommand command)
    {
        var result = await sender.Send(command);
        // Retorna um status 201 Created com a localização do novo recurso
        return Results.Created($"/api/characters/{result.Id}", result);
    }

    /// <summary>
    /// Define o personagem ativo para o usuário logado.
    /// </summary>
    private static async Task<IResult> SelectCharacter(ISender sender, int id)
    {
        await sender.Send(new SelectCharacterCommand(id));
        return Results.NoContent(); // Sucesso, sem conteúdo para retornar
    }

    /// <summary>
    /// Deleta um personagem específico.
    /// </summary>
    private static async Task<IResult> DeleteCharacter(ISender sender, int id)
    {
        await sender.Send(new DeleteCharacterCommand(id));
        return Results.NoContent();
    }

    /// <summary>
    /// Obtém a lista de personagens do usuário logado.
    /// </summary>
    private static async Task<IResult> GetMyCharacters(ISender sender)
    {
        var list = await sender.Send(new GetMyCharactersQuery());
        return Results.Ok(list);
    }

    /// <summary>
    /// Obtém os detalhes de um personagem pelo seu ID.
    /// </summary>
    private static async Task<IResult> GetCharacterById(ISender sender, int id)
    {
        var character = await sender.Send(new GetCharacterByIdQuery(id));
        return Results.Ok(character);
    }
}
