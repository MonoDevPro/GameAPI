using GameWeb.Application.Characters.Models;
using GameWeb.Application.Characters.Specifications;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;
using GameWeb.Domain.Enums;
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
        var character = Character.CreateNew(
            (CharacterName)request.Name, 
            request.Gender, 
            request.Vocation, 
            user.Id!);
        
        characterRepo.Add(character);
        
        // O UnitOfWorkBehavior irá comitar a transação.
        return Task.FromResult(mapper.Map<CharacterDto>(character));
    }
}
