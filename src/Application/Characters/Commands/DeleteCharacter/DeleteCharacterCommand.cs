using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Common.Security;
using GameWeb.Domain.Constants;
using GameWeb.Domain.Entities;

namespace GameWeb.Application.Characters.Commands.DeleteCharacter;

public record DeleteCharacterCommand(int CharacterId) : ICommand<int>;

public class DeleteCharacterCommandValidator : AbstractValidator<DeleteCharacterCommand>
{
    public DeleteCharacterCommandValidator()
    {
        RuleFor(x => x.CharacterId).GreaterThan(0);
    }
}

public class DeleteCharacterCommandHandler(IApplicationDbContext db, IUser user)
    : IRequestHandler<DeleteCharacterCommand, int>
{
    public async Task<int> Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
    {
        var character = await db.Characters.FirstOrDefaultAsync(
            c => c.Id == request.CharacterId && (c.OwnerId == user.Id),
            cancellationToken);
        
        if (character is null)
            throw new NotFoundException(nameof(Character), request.CharacterId.ToString());
        
        character.Deactivate();

        return character.Id;
    }
}
