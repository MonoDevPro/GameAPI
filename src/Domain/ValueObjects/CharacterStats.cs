namespace GameWeb.Domain.ValueObjects;

public record CharacterStats
{
    public int Id { get; init; }
    public int HealthMax { get; init; }
    public int HealthCurrent { get; private set; }
    public int AttackDamage { get; init; }
    public int AttackRange { get; init; }
    public float AttackCastTime { get; init; }
    public float AttackCooldown { get; init; }
    public float MoveSpeed { get; init; }
    
    public CharacterStats(int healthMax, int healthCurrent, int attackDamage, int attackRange, float attackCastTime, float attackCooldown, float moveSpeed)
    {
        HealthMax = healthMax;
        HealthCurrent = healthCurrent;
        AttackDamage = attackDamage;
        AttackRange = attackRange;
        AttackCastTime = attackCastTime;
        AttackCooldown = attackCooldown;
        MoveSpeed = moveSpeed;
    }

    public void ApplyDamage(int amount)
    {
        HealthCurrent = Math.Clamp(HealthCurrent - amount, 0, HealthMax);
    }

    public void ApplyHeal(int amount)
    {
        HealthCurrent = Math.Clamp(HealthCurrent + amount, 0, HealthMax);
    }
}
