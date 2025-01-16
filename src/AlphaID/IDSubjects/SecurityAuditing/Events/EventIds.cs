using Microsoft.Extensions.Logging;

namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
/// </summary>
public static class EventIds
{
    private static readonly int s_baseId = 2000;

    /// <summary>
    /// </summary>
    public static EventId CreatePersonSuccess => new(s_baseId + 0, nameof(CreatePersonSuccess));

    /// <summary>
    /// </summary>
    public static EventId CreatePersonFailure => new(s_baseId + 1, nameof(UpdatePersonFailure));

    /// <summary>
    /// </summary>
    public static EventId UpdatePersonSuccess => new(s_baseId + 2, nameof(UpdatePersonSuccess));

    /// <summary>
    /// </summary>
    public static EventId UpdatePersonFailure => new(s_baseId + 3, nameof(UpdatePersonFailure));

    /// <summary>
    /// </summary>
    public static EventId DeletePersonSuccess => new(s_baseId + 4, nameof(DeletePersonSuccess));

    /// <summary>
    /// </summary>
    public static EventId DeletePersonFailure => new(s_baseId + 5, nameof(DeletePersonFailure));

    /// <summary>
    /// </summary>
    public static EventId ChangePasswordSuccess => new(s_baseId + 6, nameof(ChangePasswordSuccess));

    /// <summary>
    /// </summary>
    public static EventId ChangePasswordFailure => new(s_baseId + 7, nameof(ChangePasswordFailure));
}