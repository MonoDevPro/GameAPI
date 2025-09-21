using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Common.Security;
using GameWeb.Domain.Constants;
using GameWeb.Domain.Entities;

namespace GameWeb.Application.Characters.Commands.DeleteCharacter;

public record DeleteCharacterCommand(int CharacterId) : ICommand;

public class DeleteCharacterCommandValidator : AbstractValidator<DeleteCharacterCommand>
{
    public DeleteCharacterCommandValidator()
    {
        RuleFor(x => x.CharacterId).GreaterThan(0);
    }
}

public class DeleteCharacterCommandHandler(IApplicationDbContext db, IUser user)
    : IRequestHandler<DeleteCharacterCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
    {
        if (user.Id is null)
            throw new UnauthorizedAccessException();
        
        var ownerIsAdmin = user.Roles?.Contains(Roles.Administrator) ?? false;

        var character = await db.Characters.FirstOrDefaultAsync(
            c => c.Id == request.CharacterId && (c.OwnerId == user.Id || ownerIsAdmin),
            cancellationToken);
        
        if (character is null)
            throw new NotFoundException(nameof(Character), request.CharacterId.ToString());

        character.Deactivate();
        character.Deselect();

        return Unit.Value;
    }
}
