using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;
using GameWeb.Application.Characters.Specifications;

namespace GameWeb.Application.Characters.Commands.SelectCharacter;

public record SelectCharacterCommand(int CharacterId) : ICommand;

public class SelectCharacterCommandValidator : AbstractValidator<SelectCharacterCommand>
{
    public SelectCharacterCommandValidator(IRepository<Character> characterRepo, IUser user)
    {
        var userId = Guard.Against.Null(user.Id, nameof(user.Id));
        
        RuleFor(x => x.CharacterId)
            .GreaterThan(0)
            .MustAsync((charId, token) => BeAValidCharacterId(charId, userId, characterRepo, token))
            .WithMessage("Character not found, is inactive, or you don't have permission to select it.");
    }
    
    private async Task<bool> BeAValidCharacterId(int characterId, string userId, IRepository<Character> characterRepo, CancellationToken cancellationToken)
    {
        var spec = new CharacterForSelectionSpec(characterId, userId);
        return await characterRepo.AnyBySpecAsync(spec, cancellationToken);
    }
}

public class SelectCharacterCommandHandler(
    IUser user,
    IIdentityService identityService)
    : IRequestHandler<SelectCharacterCommand, Unit>
{
    public async Task<Unit> Handle(SelectCharacterCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(user.Id, nameof(user.Id));
        
        // A validação já foi feita, agora executamos a ação.
        var setResult = await identityService.SetActiveCharacterAsync(user.Id, request.CharacterId, cancellationToken);
        
        if (!setResult.Succeeded)
            throw new ApplicationException($"Failed to set active character: {string.Join(", ", setResult.Errors)}");
        
        return Unit.Value;
    }
}
