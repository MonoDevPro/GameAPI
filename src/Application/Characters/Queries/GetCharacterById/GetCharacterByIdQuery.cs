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

public class GetCharacterByIdQueryHandler(IApplicationDbContext db, IUser user, IMapper mapper)
    : IRequestHandler<GetCharacterByIdQuery, CharacterDto>
{
    public async Task<CharacterDto> Handle(GetCharacterByIdQuery request, CancellationToken cancellationToken)
    {
        if (user.Id is null)
            throw new UnauthorizedAccessException();

        var character = await db.Characters.FirstOrDefaultAsync(c => c.Id == request.Id && c.OwnerId == user.Id, cancellationToken);
        if (character is null)
            throw new NotFoundException(nameof(Character), request.Id.ToString());

        return mapper.Map<CharacterDto>(character);
    }
}
