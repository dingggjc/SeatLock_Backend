namespace SeatLock.Application.Common.Exceptions;

public sealed class RequestValidationException(IReadOnlyDictionary<string, string[]> errors) : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
