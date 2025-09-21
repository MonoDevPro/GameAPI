using System.Collections.ObjectModel;

namespace GameWeb.Domain.Exceptions;

public sealed class ValidationException : DomainException
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IEnumerable<ValidationError> errors, string code = "validation.error", IDictionary<string, object>? metadata = null)
        : base(CreateMessage(errors), code, metadata)
    {
        if (errors is null) throw new ArgumentNullException(nameof(errors));
        Errors = new ReadOnlyCollection<ValidationError>(errors.ToList());
    }

    // Conveniência para criar a partir de uma única falha
    public ValidationException(string propertyName, string message, object? attemptedValue = null, string code = "validation.error", IDictionary<string, object>? metadata = null)
        : this(new[] { new ValidationError(propertyName, message, attemptedValue) }, code, metadata)
    {
    }

    private static string CreateMessage(IEnumerable<ValidationError> errors)
    {
        var list = errors?.ToList() ?? new List<ValidationError>();
        if (!list.Any()) return "Domain validation failed.";
        return "Domain validation failed: " + string.Join("; ", list.Select(e => e.ToString()));
    }
}

public sealed record ValidationError(string PropertyName, string Message, object? AttemptedValue)
{
    public override string ToString()
        => string.IsNullOrWhiteSpace(PropertyName)
            ? Message
            : $"{PropertyName}: {Message}";
}
