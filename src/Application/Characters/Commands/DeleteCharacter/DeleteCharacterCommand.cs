using GameWeb.Application.Characters.Specifications;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;

namespace GameWeb.Application.Characters.Commands.DeleteCharacter;

public record DeleteCharacterCommand(int CharacterId) : ICommand<int>;

public class DeleteCharacterCommandHandler(
    IRepository<Character> characterRepo, 
    IUser user)
    : IRequestHandler<DeleteCharacterCommand, int>
{
    public async Task<int> Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
    {
        var spec = new DeletableCharacterSpec(request.CharacterId, user);
        var character = await characterRepo.GetBySpecAsync(spec, cancellationToken);
        
        if (character is null)
            throw new NotFoundException(nameof(Character), request.CharacterId.ToString());
        
        character.Deactivate();

        // O UnitOfWorkBehavior salvará a entidade modificada.
        return character.Id;
    }
}

