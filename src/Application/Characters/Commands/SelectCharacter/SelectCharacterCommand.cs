using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Characters.Models;
using GameWeb.Application.Common.Security;
using GameWeb.Domain.Entities;

namespace GameWeb.Application.Characters.Commands.SelectCharacter;

public record SelectCharacterCommand(int CharacterId) : ICommand<CharacterDto>;

public class SelectCharacterCommandValidator : AbstractValidator<SelectCharacterCommand>
{
    public SelectCharacterCommandValidator()
    {
        RuleFor(x => x.CharacterId).GreaterThan(0);
    }
}

public class SelectCharacterCommandHandler(IApplicationDbContext db, IUser user, IMapper mapper)
    : IRequestHandler<SelectCharacterCommand, CharacterDto>
{
    public async Task<CharacterDto> Handle(SelectCharacterCommand request, CancellationToken cancellationToken)
    {
        if (user.Id is null)
            throw new UnauthorizedAccessException();
        
        var character = await db.Characters.FirstOrDefaultAsync(c => c.Id == request.CharacterId && c.OwnerId == user.Id, cancellationToken);
        if (character is null)
            throw new NotFoundException(nameof(Character), request.CharacterId.ToString());

        if (!character.IsActive)
            throw new ValidationException(new []{ new FluentValidation.Results.ValidationFailure(nameof(request.CharacterId), "Character is inactive.")});

        character.Select();

        return mapper.Map<CharacterDto>(character);
    }
}
