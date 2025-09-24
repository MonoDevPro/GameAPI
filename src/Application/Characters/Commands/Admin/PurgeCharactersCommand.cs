using GameWeb.Application.Characters.Specifications;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Common.Security;
using GameWeb.Domain.Constants;
using GameWeb.Domain.Entities;

namespace GameWeb.Application.Characters.Commands.Admin;

[Authorize(Policy = Policies.CanPurge)]
public record PurgeCharactersCommand : ICommand<int>;

public class PurgeCharactersCommandHandler(IRepository<Character> characterRepo)
    : IRequestHandler<PurgeCharactersCommand, int>
{
    public async Task<int> Handle(PurgeCharactersCommand request, CancellationToken cancellationToken)
    {
        // 1. Usa a especificação para obter a lista de personagens a serem removidos.
        var spec = new InactiveCharactersForPurgeSpec();
        var inactiveCharacters = await characterRepo.ListBySpecAsync(spec, cancellationToken);

        if (!inactiveCharacters.Any())
            return 0; // Nenhum personagem para remover.
        
        // 2. Itera sobre a lista e marca cada um para ser apagado.
        foreach (var character in inactiveCharacters)
            characterRepo.Delete(character);

        return inactiveCharacters.Count;
    }
}
