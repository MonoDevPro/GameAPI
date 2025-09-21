using GameWeb.Domain.Exceptions;

namespace GameWeb.Domain.ValueObjects;

public sealed record CharacterName
{
    public string Value { get; }

    private CharacterName(string value) => Value = value;

    public static CharacterName Create(string name)
    {
        if (!TryCreate(name, out var result))
            throw new ValidationException(
                new[] { new ValidationError(nameof(name), "Invalid character name", name) });

        return result!;
    }

    public static bool TryCreate(string? name, out CharacterName? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(name)) return false;

        var normalized = Normalize(name);
        // regras de sintaxe
        if (normalized.Length < 3 || normalized.Length > 20) return false;
        // allowed chars example: letters, numbers, spaces, underscore, hyphen
        if (!System.Text.RegularExpressions.Regex.IsMatch(normalized, @"^[\p{L}0-9 _-]+$")) return false;
        // evitar só espaços (Normalize já faz trim/collapse)
        if (string.IsNullOrWhiteSpace(normalized)) return false;

        result = new CharacterName(normalized);
        return true;
    }

    private static string Normalize(string s)
    {
        s = s.Trim();
        // collapse multiple spaces to single
        var parts = s.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        var collapsed = string.Join(' ', parts);
        // opcional: normalizar unicode (NFC/NFD) ou lower-casing if you want case-insensitive uniqueness
        return collapsed;
    }

    public override string ToString() => Value;
    
    public static implicit operator string(CharacterName name) => name.Value;
    public static explicit operator CharacterName(string name) => Create(name);
}

