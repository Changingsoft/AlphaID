using Microsoft.Extensions.Logging;

namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
/// 
/// </summary>
public static class EventIds
{
    private static readonly int BaseId = 2000;

    /// <summary>
    /// 
    /// </summary>
    public static EventId CreatePersonSuccess => new(BaseId + 1, nameof(CreatePersonSuccess));

    /// <summary>
    /// 
    /// </summary>
    public static EventId UpdatePersonSuccess => new(BaseId + 2, nameof(UpdatePersonSuccess));

    /// <summary>
    /// 
    /// </summary>
    public static EventId UpdatePersonFailure => new(BaseId + 3, nameof(UpdatePersonFailure));
}
