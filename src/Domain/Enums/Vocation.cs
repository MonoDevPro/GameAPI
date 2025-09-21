namespace GameWeb.Domain.Enums;

/// <summary>
/// Represents the different character classes available in the game
/// </summary>
public enum Vocation : int
{
    None = 0,
    
    /// <summary>
    /// Warrior class - melee combat specialist
    /// </summary>
    Warrior = 1,

    /// <summary>
    /// Mage class - magic damage dealer
    /// </summary>
    Mage = 2,

    /// <summary>
    /// Archer class - ranged physical damage dealer
    /// </summary>
    Archer = 3
}
