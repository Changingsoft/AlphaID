namespace IdSubjects.SecurityAuditing;

/// <summary>
/// </summary>
public class AuditEventsOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to raise success events.
    /// </summary>
    /// <value>
    /// <c>true</c> if success event should be raised; otherwise, <c>false</c>.
    /// </value>
    public bool RaiseSuccessEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to raise failure events.
    /// </summary>
    /// <value>
    /// <c>true</c> if failure events should be raised; otherwise, <c>false</c>.
    /// </value>
    public bool RaiseFailureEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to raise information events.
    /// </summary>
    /// <value>
    /// <c>true</c> if information events should be raised; otherwise, <c>false</c>.
    /// </value>
    public bool RaiseInformationEvents { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to raise error events.
    /// </summary>
    /// <value>
    /// <c>true</c> if error events should be raised; otherwise, <c>false</c>.
    /// </value>
    public bool RaiseErrorEvents { get; set; } = false;
}