using GameWeb.Application.Common.Interfaces;

namespace GameWeb.Infrastructure.Data;

/// <summary>
/// Implementação de IUnitOfWork usando EF Core DbContext.
/// </summary>
public class EfUnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // SaveChangesAsync retorna número de registros afetados
        var affected = await context.SaveChangesAsync(cancellationToken);
        return affected > 0;
    }
}
