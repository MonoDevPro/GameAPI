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
        if (user.Id is null)
            throw new UnauthorizedAccessException();

        return await db.Characters
            .Where(c => c.OwnerId == user.Id)
            .OrderByDescending(c => c.IsSelected)
            .ThenBy(c => c.Name)
            .AsQueryable()
            .ProjectToListAsync<CharacterDto>(mapper.ConfigurationProvider, cancellationToken);
    }
}
