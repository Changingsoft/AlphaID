using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace IdSubjects.SecurityAuditing.Events;

/// <summary>
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="category"></param>
/// <param name="eventId"></param>
/// <param name="type"></param>
/// <param name="message"></param>
public abstract class AuditLogEvent(string category, EventId eventId, AuditLogEventTypes type, string? message = null)
{
    private static readonly JsonSerializerOptions s_options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    static AuditLogEvent()
    {
        s_options.Converters.Add(new JsonStringEnumConverter());
    }

    /// <summary>
    /// 事件Id.
    /// </summary>
    public EventId EventId { get; } = eventId;

    /// <summary>
    /// 任务类别。
    /// </summary>
    public string Category { get; set; } = category;

    /// <summary>
    /// </summary>
    public AuditLogEventTypes EventType { get; set; } = type;

    /// <summary>
    /// </summary>
    public string? Message { get; set; } = message;

    /// <summary>
    /// Gets or sets the per-request activity identifier.
    /// </summary>
    /// <value>
    /// The activity identifier.
    /// </value>
    public string? ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the time stamp when the event was raised.
    /// </summary>
    /// <value>
    /// The time stamp.
    /// </value>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the server process identifier.
    /// </summary>
    /// <value>
    /// The process identifier.
    /// </value>
    public int ProcessId { get; set; }

    /// <summary>
    /// Gets or sets the local ip address of the current request.
    /// </summary>
    /// <value>
    /// The local ip address.
    /// </value>
    public string? LocalIpAddress { get; set; }

    /// <summary>
    /// Gets or sets the remote ip address of the current request.
    /// </summary>
    /// <value>
    /// The remote ip address.
    /// </value>
    public string? RemoteIpAddress { get; set; }

    /// <summary>
    /// Allows implementing custom initialization logic.
    /// </summary>
    /// <returns></returns>
    protected internal virtual Task PrepareAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obfuscates a token.
    /// </summary>
    /// <param name="value">The token.</param>
    /// <returns></returns>
    protected static string Obfuscate(string value)
    {
        var last4Chars = "****";
        if (!string.IsNullOrWhiteSpace(value) && value.Length > 4) last4Chars = value[^4..];

        return "****" + last4Chars;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, s_options);
    }
}