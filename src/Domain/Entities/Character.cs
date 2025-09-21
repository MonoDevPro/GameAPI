namespace GameWeb.Domain.Entities;

public class Character : BaseAuditableEntity
{
    // 2. Encapsulamento: Proteja as propriedades que não devem ser alteradas livremente.
    // Use 'private set' para que só possam ser modificadas de dentro da classe.
    public CharacterName Name { get; private set; }
    public Gender Gender { get; private set; }
    public Vocation Vocation { get; private set; }

    // 3. Agrupamento com Value Objects: Agrupe conceitos relacionados.
    public CharacterStats Stats { get; private set; }
    public Vector2D Position { get; private set; }
    public Vector2D Direction { get; private set; }
    
    // Indica se este personagem está atualmente selecionado pelo usuário (ativo na sessão de jogo)
    
    // Propriedades que precisam ser públicas para o EF Core mapear a chave estrangeira.
    public string OwnerId { get; private set; } = null!;

    // 4. Construtor: Garanta que um personagem SEMPRE seja criado em um estado válido.
    // Isso força a criação de um personagem com os dados mínimos necessários.
    private Character(
        CharacterName name, 
        Gender gender, 
        Vocation vocation, 
        string ownerId,
        CharacterStats stats,
        Vector2D position,
        Vector2D direction)
    {
        Name = name;
        Gender = gender;
        Vocation = vocation;
        OwnerId = ownerId;
        Stats = stats;
        Position = position;
        Direction = direction;
    }

    // 5. Factory Method (Método de Fábrica): Uma forma limpa de instanciar.
    // Aqui você pode definir os valores iniciais baseados na vocação, por exemplo.
    public static Character CreateNew(CharacterName name, Gender gender, Vocation vocation, string ownerId)
    {
        // Lógica para definir stats iniciais baseados na vocação
        var initialStats = vocation switch
        {
            Vocation.Warrior => new CharacterStats(150, 150, 15, 1, 1.2f, 1.5f, 1.0f),
            Vocation.Mage => new CharacterStats(100, 100, 20, 5, 2.0f, 2.0f, 1.1f),
            _ => new CharacterStats(120, 120, 10, 1, 1.0f, 1.0f, 1.2f)
        };
        
        var initialPosition = new Vector2D(0, 0); // Posição inicial padrão
        var initialDirection = new Vector2D(0, 1); // Olhando para "baixo"

        var character = new Character(name, gender, vocation, ownerId, initialStats, initialPosition, initialDirection);
        character.AddDomainEvent(new CharacterCreatedEvent(character.Id));
        return character;
    }

    public override void Deactivate()
    {
        var wasActive = IsActive;
        base.Deactivate();
        if (wasActive && !IsActive)
        {
            AddDomainEvent(new CharacterDeactivatedEvent(Id));
        }
    }

    public override void Activate()
    {
        var wasInactive = !IsActive;
        base.Activate();
        if (wasInactive && IsActive)
        {
            AddDomainEvent(new CharacterActivatedEvent(Id));
        }
    }

    // 6. Comportamento Explícito: Em vez de setters públicos, crie métodos que descrevem a ação.
    public void TakeDamage(int damageAmount)
    {
        if (damageAmount <= 0) return;
        
        // A lógica de negócio (vida não pode ser < 0) está protegida aqui.
        Stats.ApplyDamage(damageAmount);
    }
    
    public void Heal(int healAmount)
    {
        if (healAmount <= 0) return;

        // A lógica de negócio (vida não pode ser > vida máxima) está protegida aqui.
        Stats.ApplyHeal(healAmount);
    }

    public void MoveTo(Vector2D newPosition)
    {
        // Aqui você poderia adicionar lógica de validação de movimento, se necessário.
        Position = newPosition;
    }

    // Construtor privado para o EF Core.
    // O EF Core precisa de um construtor sem parâmetros para materializar a entidade do banco.
    // Torná-lo privado impede o uso indevido fora do contexto do ORM.
#pragma warning disable CS8618
    private Character() { }
#pragma warning restore CS8618
}
