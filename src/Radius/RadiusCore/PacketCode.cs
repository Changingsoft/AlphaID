namespace RadiusCore;

/// <summary>
/// Packet codes for RADIUS packets.
/// </summary>
public enum PacketCode : byte
{
    /// <summary>
    /// Access-Request (1)
    /// </summary>
    AccessRequest = 1,
    /// <summary>
    /// Access-Accept (2)
    /// </summary>
    AccessAccept = 2,
    /// <summary>
    /// Access-Reject (3)
    /// </summary>
    AccessReject = 3,
    /// <summary>
    /// Accounting-Request (4)
    /// </summary>
    AccountingRequest = 4,
    /// <summary>
    /// Accounting-Response (5)
    /// </summary>
    AccountingResponse = 5,
    /// <summary>
    /// Access-Challenge (11)
    /// </summary>
    AccessChallenge = 11,
    /// <summary>
    /// Status-Server (12)
    /// </summary>
    StatusServer = 12,
    /// <summary>
    /// Status-Client (13)
    /// </summary>
    StatusClient = 13,
    /// <summary>
    /// 
    /// </summary>
    DisconnectRequest = 40,
    /// <summary>
    /// 
    /// </summary>
    DisconnectAck = 41,
    /// <summary>
    /// 
    /// </summary>
    DisconnectNak = 42,
    /// <summary>
    /// 
    /// </summary>
    CoaRequest = 43,
    /// <summary>
    /// 
    /// </summary>
    CoaAck = 44,
    /// <summary>
    /// 
    /// </summary>
    CoaNak = 45
}