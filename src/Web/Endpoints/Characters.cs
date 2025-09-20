using GameWeb.Application.Characters.Commands.CreateCharacter;
using GameWeb.Application.Characters.Commands.SelectCharacter;
using GameWeb.Application.Characters.Commands.DeleteCharacter;
using GameWeb.Application.Characters.Queries.GetMyCharacters;
using GameWeb.Application.Characters.Queries.GetCharacterById;
using GameWeb.Application.Characters.Models;
using GameWeb.Application.Common.Security;

namespace GameWeb.Web.Endpoints;

public class Characters : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGroup("characters");

        group.MapPost("/", async (CreateCharacterCommand cmd, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(cmd, ct);
            return Results.Created($"/api/characters/{result.Id}", result);
        });

        group.MapPost("/{id:int}/select", async (int id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SelectCharacterCommand(id), ct);
            return Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteCharacterCommand(id), ct);
            return Results.NoContent();
        });

        group.MapGet("/me", async (ISender sender, CancellationToken ct) =>
        {
            var list = await sender.Send(new GetMyCharactersQuery(), ct);
            return Results.Ok(list);
        });

        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
        {
            var character = await sender.Send(new GetCharacterByIdQuery(id), ct);
            return Results.Ok(character);
        });
    }
}
