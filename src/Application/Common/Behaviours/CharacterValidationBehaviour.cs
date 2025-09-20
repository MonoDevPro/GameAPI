using System.Reflection;
using GameWeb.Application.Common.Exceptions;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Application.Common.Security;
using Microsoft.Extensions.Logging;

namespace GameWeb.Application.Common.Behaviours;

public class CharacterValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUser _currentUser;
    private readonly IApplicationDbContext _db;
    private readonly ICharacterClaimSetter _claimSetter;
    private readonly ILogger<CharacterValidationBehaviour<TRequest, TResponse>> _logger;

    public CharacterValidationBehaviour(
        IUser currentUser,
        IApplicationDbContext db,
        ICharacterClaimSetter claimSetter,
        ILogger<CharacterValidationBehaviour<TRequest, TResponse>> logger)
    {
        _currentUser = currentUser;
        _db = db;
        _claimSetter = claimSetter;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attr = request.GetType().GetCustomAttribute<CharacterRequiredAttribute>();
        if (attr is null)
        {
            return await next();
        }

        if (_currentUser.Id is null)
        {
            throw new UnauthorizedAccessException();
        }

        // Se já existe claim character_id, apenas segue (assume previamente validado no ciclo)
        if (_currentUser.CharacterId.HasValue)
        {
            return await next();
        }

        // Carregar usuário para obter ActiveCharacterId (se for persistido no futuro) ou descobrir personagem selecionado.
        // Nesta primeira abordagem, buscamos o Character marcado como IsSelected.
        var selectedCharacter = await _db.Characters
            .AsNoTracking()
            .Where(c => c.OwnerId == _currentUser.Id && c.IsSelected && c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (selectedCharacter is null)
        {
            throw new NoCharacterSelectedException();
        }

        if (!attr.AllowNotOwner && selectedCharacter.OwnerId != _currentUser.Id)
        {
            throw new ForbiddenAccessException();
        }

        // Solicita inclusão da claim via serviço externo (Web layer)
        _claimSetter.EnsureCharacterClaim(selectedCharacter.Id);

        _logger.LogDebug("Character validation succeeded for user {UserId} with character {CharacterId}", _currentUser.Id, selectedCharacter.Id);

        return await next();
    }
}
