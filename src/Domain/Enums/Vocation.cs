namespace GameWeb.Domain.Enums;

/// <summary>
/// Represents the different character classes available in the game
/// </summary>
public enum Vocation : int
{
    None = -1,
    
    /// <summary>
    /// Warrior class - melee combat specialist
    /// </summary>
    Warrior = 0,

    /// <summary>
    /// Mage class - magic damage dealer
    /// </summary>
    Mage = 1,

    /// <summary>
    /// Archer class - ranged physical damage dealer
    /// </summary>
    Archer = 2
}
