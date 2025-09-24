using GameWeb.Application.Characters.Models;
using GameWeb.Application.Characters.Specifications;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;
using GameWeb.Domain.Enums;
using GameWeb.Domain.Events;
using GameWeb.Domain.ValueObjects;

namespace GameWeb.Application.Characters.Commands.CreateCharacter;

public record CreateCharacterCommand(string Name, Gender Gender, Vocation Vocation) : ICommand<CharacterDto>;

public class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterCommandValidator(IRepository<Character> characterRepo, IUser user)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(20)
            .MustAsync(async (name, cancellationToken) => 
            {
                // Usa a nova especificação para a verificação. A intenção fica muito clara.
                var spec = new CharacterByNameSpec(name);
                return !await characterRepo.AnyAsync(spec, cancellationToken);
            })
            .WithMessage("Character name already exists.");
        
        RuleFor(x => x.Gender).IsInEnum().NotEqual(Gender.None);
        RuleFor(x => x.Vocation).IsInEnum().NotEqual(Vocation.None);
    }
}

public class CreateCharacterCommandHandler(
    IRepository<Character> characterRepo, 
    IUser user, 
    IMapper mapper)
    : IRequestHandler<CreateCharacterCommand, CharacterDto>
{
    public Task<CharacterDto> Handle(CreateCharacterCommand request, CancellationToken cancellationToken)
    {
        var character = new Character
        {
            Name = request.Name, 
            Gender = request.Gender, 
            Vocation = request.Vocation, 
            Stats = request.Vocation switch
            {
                Vocation.Warrior => new CharacterStats(0, 150, 150, 20, 1, 0.5f, 1.0f, 3.5f),
                Vocation.Archer => new CharacterStats(0, 100, 100, 15, 5, 0.3f, 0.7f, 4.0f),
                Vocation.Mage => new CharacterStats(0, 80, 80, 25, 4, 1.0f, 2.0f, 3.0f),
                _ => throw new ArgumentOutOfRangeException()
            },
            Position = new Vector2D (5, 5),
            Direction = new Vector2D(0,1),
            OwnerId = user.Id!
        };
        character.AddDomainEvent(new CharacterCreatedEvent(character.Id));
        
        characterRepo.Add(character);
        
        
        // O UnitOfWorkBehavior irá comitar a transação.
        return Task.FromResult(mapper.Map<CharacterDto>(character));
    }
}
