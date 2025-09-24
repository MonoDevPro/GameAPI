using GameWeb.Application.Characters.Models;
using GameWeb.Application.Characters.Specifications;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;

namespace GameWeb.Application.Characters.Queries.GetSelectedCharacter;

public record GetSelectedCharacterQuery : IQuery<CharacterDto?>;

public class GetSelectedCharacterQueryHandler(
    IRepository<Character> characterRepo, 
    IIdentityService identityService,
    IUser user)
    : IRequestHandler<GetSelectedCharacterQuery, CharacterDto?>
{
    public async Task<CharacterDto?> Handle(GetSelectedCharacterQuery request, CancellationToken cancellationToken)
    {
        // 1. Usa a abstração para obter o ID do personagem ativo.
        var activeCharacterId = await identityService.GetActiveCharacterIdAsync(user.Id!, cancellationToken);

        if (activeCharacterId is null)
            return null;

        // 2. Usa o ID para buscar o personagem através do repositório.
        var spec = new CharacterByIdSpec(activeCharacterId.Value);
        var character = await characterRepo.GetBySpecAsync<CharacterDto>(spec, cancellationToken);

        return character ?? throw new NotFoundException(nameof(Character), activeCharacterId.Value.ToString());
    }
}
