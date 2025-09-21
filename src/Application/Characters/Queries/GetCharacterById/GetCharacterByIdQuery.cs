using GameWeb.Application.Characters.Models;
using GameWeb.Application.Characters.Specifications;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Common.Mappings;
using GameWeb.Domain.Entities;

namespace GameWeb.Application.Characters.Queries.GetCharacterById;

public record GetCharacterByIdQuery(int Id) : IQuery<CharacterDto>;

public class GetCharacterByIdQueryHandler(
    IRepository<Character> characterRepo, 
    IUser user)
    : IRequestHandler<GetCharacterByIdQuery, CharacterDto>
{
    public async Task<CharacterDto> Handle(GetCharacterByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new CharacterByIdSpec(request.Id, user.Id!);
        var character = await characterRepo.GetBySpecAsync<CharacterDto>(spec, cancellationToken);
        
        return character ?? throw new NotFoundException(nameof(Character), request.Id.ToString());
    }
}
