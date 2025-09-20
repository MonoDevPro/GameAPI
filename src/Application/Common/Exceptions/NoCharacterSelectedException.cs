namespace GameWeb.Application.Common.Exceptions;

public class NoCharacterSelectedException : Exception
{
    public NoCharacterSelectedException() : base("No active character selected for current user.") { }
    public NoCharacterSelectedException(string? message) : base(message) { }
}
