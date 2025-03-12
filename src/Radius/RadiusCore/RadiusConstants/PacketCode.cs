namespace RadiusCore.RadiusConstants
{
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
        DisconnectRequest = 40,
        DisconnectAck = 41,
        DisconnectNak = 42,
        CoaRequest = 43,
        CoaAck = 44,
        CoaNak = 45
    }
}