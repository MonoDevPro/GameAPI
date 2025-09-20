using GameWeb.Application.Common.Interfaces;
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

public class DeleteCharacterCommandHandler : IRequestHandler<DeleteCharacterCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public DeleteCharacterCommandHandler(IApplicationDbContext db, IUser user)
    {
        _db = db;
        _user = user;
    }

    public async Task<Unit> Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
    {
        if (_user.Id is null)
            throw new UnauthorizedAccessException();

        var character = await _db.Characters.FirstOrDefaultAsync(c => c.Id == request.CharacterId && c.OwnerId == _user.Id, cancellationToken);
        if (character is null)
            throw new NotFoundException(nameof(Character), request.CharacterId.ToString());

        character.Deactivate();
        character.Deselect();
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
