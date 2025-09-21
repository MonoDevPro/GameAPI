using GameWeb.Application.Characters.Models;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Common.Mappings;
using GameWeb.Application.Common.Models;
using GameWeb.Application.Common.Security;
using GameWeb.Domain.Constants;

namespace GameWeb.Application.Characters.Queries.Admin;

[Authorize(Roles = Roles.Administrator)]
public record GetAllCharactersQuery(bool? IsActive = null, int PageNumber = 1, int PageSize = 10) : IQuery<PaginatedList<CharacterDto>>;

public class GetAllCharactersQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetAllCharactersQuery, PaginatedList<CharacterDto>>
{
    public async Task<PaginatedList<CharacterDto>> Handle(GetAllCharactersQuery request, CancellationToken cancellationToken)
    {
        var query = db.Characters
            .IgnoreQueryFilters()
            .OrderBy(c => c.Name)
            .AsQueryable();
        
        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        
        return await query
            .ProjectTo<CharacterDto>(mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
