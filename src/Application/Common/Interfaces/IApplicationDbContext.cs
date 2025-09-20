using GameWeb.Domain.Entities;

namespace GameWeb.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Character> Characters { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
