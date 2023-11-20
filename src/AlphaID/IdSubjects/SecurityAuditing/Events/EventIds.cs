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
    public static EventId CreatePersonSuccess => new(BaseId + 0, nameof(CreatePersonSuccess));
    /// <summary>
    /// 
    /// </summary>
    public static EventId CreatePersonFailure => new(BaseId + 1, nameof(UpdatePersonFailure));

    /// <summary>
    /// 
    /// </summary>
    public static EventId UpdatePersonSuccess => new(BaseId + 2, nameof(UpdatePersonSuccess));

    /// <summary>
    /// 
    /// </summary>
    public static EventId UpdatePersonFailure => new(BaseId + 3, nameof(UpdatePersonFailure));

    /// <summary>
    /// 
    /// </summary>
    public static EventId DeletePersonSuccess => new(BaseId + 4, nameof(DeletePersonSuccess));

    /// <summary>
    /// 
    /// </summary>
    public static EventId DeletePersonFailure => new(BaseId + 5, nameof(DeletePersonFailure));

    /// <summary>
    /// 
    /// </summary>
    public static EventId ChangePasswordSuccess => new (BaseId + 6, nameof(ChangePasswordSuccess));

    /// <summary>
    /// 
    /// </summary>
    public static EventId ChangePasswordFailure => new (BaseId + 7, nameof(ChangePasswordFailure));
}
