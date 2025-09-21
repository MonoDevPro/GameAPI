using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Characters.Models;
using GameWeb.Domain.Entities;
using FluentValidation; // Para a ValidationException
using FluentValidation.Results;
using GameWeb.Application.Characters.Specifications;
using GameWeb.Application.Common.Mappings; // Para o ValidationFailure

namespace GameWeb.Application.Characters.Commands.SelectCharacter;

public record SelectCharacterCommand(int CharacterId) : ICommand<CharacterDto>;

public class SelectCharacterCommandValidator : AbstractValidator<SelectCharacterCommand>
{
    public SelectCharacterCommandValidator(IRepository<Character> characterRepo, IUser user)
    {
        RuleFor(x => x.CharacterId)
            .GreaterThan(0)
            .MustAsync(async (id, cancellationToken) =>
            {
                var spec = new CharacterForSelectionSpec(id, user.Id!);
                return await characterRepo.AnyAsync(spec, cancellationToken);
            })
            .WithMessage("Character not found, is inactive, or you don't have permission to select it.");
    }
}

public class SelectCharacterCommandHandler(
    IRepository<Character> characterRepo,
    IUser user,
    IIdentityService identityService)
    : IRequestHandler<SelectCharacterCommand, CharacterDto>
{
    public async Task<CharacterDto> Handle(SelectCharacterCommand request, CancellationToken cancellationToken)
    {
        // A validação já foi feita, agora executamos a ação.
        var setResult = await identityService.SetActiveCharacterAsync(user.Id!, request.CharacterId, cancellationToken);
        
        if (!setResult.Succeeded)
            throw new ApplicationException($"Failed to set active character: {string.Join(", ", setResult.Errors)}");

        // Retorna o DTO do personagem que foi selecionado.
        var spec = new CharacterByIdSpec(request.CharacterId);
        var characterDto = await characterRepo.GetBySpecAsync<CharacterDto>(spec, cancellationToken);

        return characterDto ?? throw new NotFoundException(nameof(Character), request.CharacterId.ToString());
    }
}
