// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace AuthCenterWebApp.Pages;

internal static class Log
{
    private static readonly Action<ILogger, string?, Exception?> s_invalidId = LoggerMessage.Define<string?>(
        LogLevel.Error,
        EventIds.InvalidId,
        "Invalid id {Id}");

    private static readonly Action<ILogger, string?, Exception?> s_invalidBackchannelLoginId =
        LoggerMessage.Define<string?>(
            LogLevel.Warning,
            EventIds.InvalidBackchannelLoginId,
            "Invalid backchannel login id {Id}");

    private static readonly Action<ILogger, IEnumerable<string>, Exception?> s_externalClaims =
        LoggerMessage.Define<IEnumerable<string>>(
            LogLevel.Debug,
            EventIds.ExternalClaims,
            "External claims: {Claims}");

    private static readonly Action<ILogger, string, Exception?> s_noMatchingBackchannelLoginRequest =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            EventIds.NoMatchingBackchannelLoginRequest,
            "No backchannel login request matching id: {Id}");

    private static readonly Action<ILogger, string, Exception?> s_noConsentMatchingRequest =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            EventIds.NoConsentMatchingRequest,
            "No consent request matching request: {ReturnUrl}");

    public static void InvalidId(this ILogger logger, string? id)
    {
        s_invalidId(logger, id, null);
    }

    public static void InvalidBackchannelLoginId(this ILogger logger, string? id)
    {
        s_invalidBackchannelLoginId(logger, id, null);
    }

    public static void ExternalClaims(this ILogger logger, IEnumerable<string> claims)
    {
        s_externalClaims(logger, claims, null);
    }

    public static void NoMatchingBackchannelLoginRequest(this ILogger logger, string id)
    {
        s_noMatchingBackchannelLoginRequest(logger, id, null);
    }

    public static void NoConsentMatchingRequest(this ILogger logger, string returnUrl)
    {
        s_noConsentMatchingRequest(logger, returnUrl, null);
    }
}

internal static class EventIds
{
    private const int UIEventsStart = 10000;

    //////////////////////////////
    // Consent
    //////////////////////////////
    private const int ConsentEventsStart = UIEventsStart + 1000;
    public const int InvalidId = ConsentEventsStart + 0;
    public const int NoConsentMatchingRequest = ConsentEventsStart + 1;

    //////////////////////////////
    // External Login
    //////////////////////////////
    private const int ExternalLoginEventsStart = UIEventsStart + 2000;
    public const int ExternalClaims = ExternalLoginEventsStart + 0;

    //////////////////////////////
    // CIBA
    //////////////////////////////
    private const int CibaEventsStart = UIEventsStart + 3000;
    public const int InvalidBackchannelLoginId = CibaEventsStart + 0;
    public const int NoMatchingBackchannelLoginRequest = CibaEventsStart + 1;
}