using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Characters.Models;
using GameWeb.Domain.Entities;
using FluentValidation; // Para a ValidationException
using FluentValidation.Results;
using GameWeb.Application.Common.Mappings; // Para o ValidationFailure

namespace GameWeb.Application.Characters.Commands.SelectCharacter;

public record SelectCharacterCommand(int CharacterId) : ICommand<CharacterDto>;

public class SelectCharacterCommandValidator : AbstractValidator<SelectCharacterCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    
    public SelectCharacterCommandValidator(IApplicationDbContext db, IUser user)
    {
        if (user.Id is null)
            throw new UnauthorizedAccessException();
        
        _db = db;
        _user = user;
        
        RuleFor(x => x.CharacterId)
            .GreaterThan(0)
            .MustAsync(CharacterExists);
    }
    
    private async Task<bool> CharacterExists(int characterId, CancellationToken cancellationToken)
    {
        return await _db.Characters.AnyAsync(c => 
            c.Id == characterId 
            && c.OwnerId == _user.Id, 
            cancellationToken);
    }
}

// O Handler agora injeta IIdentityService
public class SelectCharacterCommandHandler(
    IApplicationDbContext db, 
    IUser user, 
    IIdentityService identityService, // Injeção do serviço de identidade
    IMapper mapper)
    : IRequestHandler<SelectCharacterCommand, CharacterDto>
{
    public async Task<CharacterDto> Handle(SelectCharacterCommand request, CancellationToken cancellationToken)
    {
        var setResult = await identityService.SetActiveCharacterAsync(user.Id!, request.CharacterId, cancellationToken);
        
        if (!setResult.Succeeded)
            throw new ApplicationException($"Failed to set active character: {string.Join(", ", setResult.Errors)}");
        
        return await db.Characters
            .Where(c => c.Id == request.CharacterId)
            .ProjectToFirstOrDefaultAsync<CharacterDto>(mapper, cancellationToken)
            ?? throw new NotFoundException(nameof(Character), request.CharacterId.ToString());
    }
}
