using GameWeb.Application.Characters.Models;
using GameWeb.Application.Common.Interfaces;
using GameWeb.Domain.Entities;
using GameWeb.Domain.Enums;
using GameWeb.Domain.ValueObjects;

namespace GameWeb.Application.Characters.Commands.CreateCharacter;

public record CreateCharacterCommand(string Name, Gender Gender, Vocation Vocation) : ICommand<CharacterDto>;

public class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
    private readonly IApplicationDbContext _db;

    public CreateCharacterCommandValidator(IApplicationDbContext db, IUser user)
    {
        if (user.Id is null)
            throw new UnauthorizedAccessException();
        
        _db = db;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(20)
            // A validação assíncrona é encadeada aqui
            .MustAsync(BeUniqueName)
            .WithMessage("Character name already exists.");
        
        RuleFor(x => x.Gender)
            .IsInEnum()
            .NotEqual(Gender.None);
        
        RuleFor(x => x.Vocation)
            .IsInEnum()
            .NotEqual(Vocation.None);
    }

    /// <summary>
    /// Verifica no banco de dados se o nome do personagem já está em uso.
    /// </summary>
    /// <param name="name">O nome do personagem a ser verificado.</param>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>Retorna 'true' se o nome for único, e 'false' caso contrário.</returns>
    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        // A lógica correta é verificar se NÃO existe NENHUM personagem com este nome.
        // Comparamos o `name` (string) com a propriedade `Value` do Value Object `CharacterName`.
        return !await _db.Characters
            .AnyAsync(c => c.Name == name, cancellationToken);
    }
}

public class CreateCharacterCommandHandler(IApplicationDbContext db, IUser user, IMapper mapper)
    : IRequestHandler<CreateCharacterCommand, CharacterDto>
{
    public Task<CharacterDto> Handle(CreateCharacterCommand request, CancellationToken cancellationToken)
    {
        var character = Character.CreateNew((CharacterName)request.Name, request.Gender, request.Vocation, user.Id!);
        db.Characters.Add(character);
        
        return Task.FromResult(mapper.Map<CharacterDto>(character));
    }
}
