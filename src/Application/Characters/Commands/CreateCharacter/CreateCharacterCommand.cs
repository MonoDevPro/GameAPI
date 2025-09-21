using GameWeb.Application.Characters.Models;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;
using GameWeb.Domain.Enums;
using GameWeb.Domain.ValueObjects;

namespace GameWeb.Application.Characters.Commands.CreateCharacter;

public record CreateCharacterCommand(string Name, Gender Gender, Vocation Vocation) : ICommand<CharacterDto>;

public class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(20);
        
        RuleFor(x => x.Gender)
            .IsInEnum()
            .NotEqual(Gender.None);
        
        RuleFor(x => x.Vocation)
            .IsInEnum()
            .NotEqual(Vocation.None);
    }
}

public class CreateCharacterCommandHandler(IApplicationDbContext db, IUser user, IMapper mapper)
    : IRequestHandler<CreateCharacterCommand, CharacterDto>
{
    public async Task<CharacterDto> Handle(CreateCharacterCommand request, CancellationToken cancellationToken)
    {
        if (user.Id is null)
            throw new UnauthorizedAccessException();

        // Garantir nome único
        var exists = await db.Characters.AnyAsync(c => c.Name == request.Name, cancellationToken);
        if (exists)
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(nameof(request.Name), "Character name already exists.")
            ]);

        var character = Character.CreateNew((CharacterName)request.Name, request.Gender, request.Vocation, user.Id);
        db.Characters.Add(character);

        return mapper.Map<CharacterDto>(character);
    }
}
