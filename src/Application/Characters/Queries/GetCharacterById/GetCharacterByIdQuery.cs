using GameWeb.Application.Characters.Models;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;

namespace GameWeb.Application.Characters.Queries.GetCharacterById;

public record GetCharacterByIdQuery(int Id) : IQuery<CharacterDto>;

public class GetCharacterByIdQueryValidator : AbstractValidator<GetCharacterByIdQuery>
{
    public GetCharacterByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class GetCharacterByIdQueryHandler : IRequestHandler<GetCharacterByIdQuery, CharacterDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IMapper _mapper;

    public GetCharacterByIdQueryHandler(IApplicationDbContext db, IUser user, IMapper mapper)
    {
        _db = db;
        _user = user;
        _mapper = mapper;
    }

    public async Task<CharacterDto> Handle(GetCharacterByIdQuery request, CancellationToken cancellationToken)
    {
        if (_user.Id is null)
            throw new UnauthorizedAccessException();

        var character = await _db.Characters.FirstOrDefaultAsync(c => c.Id == request.Id && c.OwnerId == _user.Id, cancellationToken);
        if (character is null)
            throw new NotFoundException(nameof(Character), request.Id.ToString());

        return _mapper.Map<CharacterDto>(character);
    }
}
