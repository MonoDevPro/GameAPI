using GameWeb.Application.Characters.Models;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Common.Mappings;

namespace GameWeb.Application.Characters.Queries.GetMyCharacters;

public record GetMyCharactersQuery : IQuery<List<CharacterDto>>;

public class GetMyCharactersQueryHandler(IApplicationDbContext db, IUser user, IMapper mapper)
    : IRequestHandler<GetMyCharactersQuery, List<CharacterDto>>
{
    public async Task<List<CharacterDto>> Handle(GetMyCharactersQuery request, CancellationToken cancellationToken)
    {
        var characters = await db.Characters
            .Where(c => c.OwnerId == user.Id)
            .ProjectToListAsync<CharacterDto>(mapper, cancellationToken);

        return characters;
    }
}
