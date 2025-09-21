using System.Linq.Expressions;
using GameWeb.Application.Common.Models;
using GameWeb.Domain.Common;

namespace GameWeb.Application.Common.Interfaces;

public interface ISpecificationRepository<T> where T : BaseEntity
{
    Task<T?> GetBySpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<List<T>> ListBySpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<List<TResult>> ListBySpecAsync<TResult>(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<PaginatedList<T>> ListBySpecAsync(ISpecification<T> spec, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PaginatedList<TResult>> ListBySpecAsync<TResult>(ISpecification<T> spec, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
