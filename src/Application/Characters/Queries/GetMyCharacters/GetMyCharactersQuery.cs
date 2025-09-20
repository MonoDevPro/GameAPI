using GameWeb.Application.Characters.Models;
using GameWeb.Application.Common.Interfaces;

namespace GameWeb.Application.Characters.Queries.GetMyCharacters;

public record GetMyCharactersQuery : IQuery<List<CharacterDto>>;

public class GetMyCharactersQueryHandler : IRequestHandler<GetMyCharactersQuery, List<CharacterDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IMapper _mapper;

    public GetMyCharactersQueryHandler(IApplicationDbContext db, IUser user, IMapper mapper)
    {
        _db = db;
        _user = user;
        _mapper = mapper;
    }

    public async Task<List<CharacterDto>> Handle(GetMyCharactersQuery request, CancellationToken cancellationToken)
    {
        if (_user.Id is null)
            throw new UnauthorizedAccessException();

        var chars = await _db.Characters
            .Where(c => c.OwnerId == _user.Id)
            .OrderByDescending(c => c.IsSelected)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return chars.Select(c => _mapper.Map<CharacterDto>(c)).ToList();
    }
}
