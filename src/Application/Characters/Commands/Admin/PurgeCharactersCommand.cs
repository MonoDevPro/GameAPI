using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Common.Security;
using GameWeb.Domain.Constants;

namespace GameWeb.Application.Characters.Commands.Admin;

[Authorize(Roles = Roles.Administrator )]
public record PurgeCharactersCommand() : ICommand<int>;

public class PurgeCharactersCommandHandler(IApplicationDbContext db)
    : IRequestHandler<PurgeCharactersCommand, int>
{
    public async Task<int> Handle(PurgeCharactersCommand request, CancellationToken cancellationToken)
    {
        var character = await db.Characters.IgnoreQueryFilters().ToListAsync(cancellationToken);
        var count = 0;
        foreach (var c in character)
        {
            if (c.IsActive) continue;
            db.Characters.Remove(c);
            count++;
        }
        return count;
    }
}
