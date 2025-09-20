using GameWeb.Application.Characters.Models;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;
using GameWeb.Domain.Enums;

namespace GameWeb.Application.Characters.Commands.CreateCharacter;

public record CreateCharacterCommand(string Name, Gender Gender, Vocation Vocation) : ICommand<CharacterDto>;

public class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public class CreateCharacterCommandHandler : IRequestHandler<CreateCharacterCommand, CharacterDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IMapper _mapper;

    public CreateCharacterCommandHandler(IApplicationDbContext db, IUser user, IMapper mapper)
    {
        _db = db;
        _user = user;
        _mapper = mapper;
    }

    public async Task<CharacterDto> Handle(CreateCharacterCommand request, CancellationToken cancellationToken)
    {
        if (_user.Id is null)
            throw new UnauthorizedAccessException();

        // Garantir nome único
        var exists = await _db.Characters.AnyAsync(c => c.Name == request.Name, cancellationToken);
        if (exists)
            throw new ValidationException(new []{ new FluentValidation.Results.ValidationFailure(nameof(request.Name), "Character name already exists.")});

        var character = Character.CreateNew(request.Name, request.Gender, request.Vocation, _user.Id);

        // Se não há nenhum selecionado ainda para esse usuário, seleciona este
        var alreadySelected = await _db.Characters.AnyAsync(c => c.OwnerId == _user.Id && c.IsSelected, cancellationToken);
        if (!alreadySelected)
        {
            character.Select();
        }

        _db.Characters.Add(character);
        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CharacterDto>(character);
    }
}
