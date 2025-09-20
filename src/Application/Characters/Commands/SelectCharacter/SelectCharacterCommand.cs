using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Characters.Models;
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

public class SelectCharacterCommandHandler : IRequestHandler<SelectCharacterCommand, CharacterDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IMapper _mapper;

    public SelectCharacterCommandHandler(IApplicationDbContext db, IUser user, IMapper mapper)
    {
        _db = db;
        _user = user;
        _mapper = mapper;
    }

    public async Task<CharacterDto> Handle(SelectCharacterCommand request, CancellationToken cancellationToken)
    {
        if (_user.Id is null)
            throw new UnauthorizedAccessException();

        var character = await _db.Characters.FirstOrDefaultAsync(c => c.Id == request.CharacterId && c.OwnerId == _user.Id, cancellationToken);
        if (character is null)
            throw new NotFoundException(nameof(Character), request.CharacterId.ToString());

        if (!character.IsActive)
            throw new ValidationException(new []{ new FluentValidation.Results.ValidationFailure(nameof(request.CharacterId), "Character is inactive.")});

        // Remover seleção dos outros
        var others = await _db.Characters.Where(c => c.OwnerId == _user.Id && c.IsSelected && c.Id != character.Id).ToListAsync(cancellationToken);
        foreach (var o in others)
        {
            o.Deselect();
        }

        character.Select();
        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CharacterDto>(character);
    }
}
