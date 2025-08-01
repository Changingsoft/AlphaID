// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.Diagnostics.Metrics;

namespace AuthCenterWebApp.Pages;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1724 // Type names should not match namespaces

/// <summary>
/// Telemetry helpers for the UI
/// </summary>
public static class Telemetry
{
    private static readonly string s_serviceVersion = typeof(Telemetry).Assembly.GetName().Version!.ToString();

    /// <summary>
    /// Service name for telemetry.
    /// </summary>
    public static readonly string ServiceName = typeof(Telemetry).Assembly.GetName().Name!;

    /// <summary>
    /// Metrics configuration
    /// </summary>
    public static class Metrics
    {
        /// <summary>
        /// Meter for the IdentityServer host project
        /// </summary>
        private static readonly Meter s_meter = new(ServiceName, s_serviceVersion);

        private static readonly Counter<long> s_consentCounter = s_meter.CreateCounter<long>(Counters.Consent);

        private static readonly Counter<long> s_grantsRevokedCounter =
            s_meter.CreateCounter<long>(Counters.GrantsRevoked);

        private static readonly Counter<long> s_userLoginCounter = s_meter.CreateCounter<long>(Counters.UserLogin);

        private static readonly Counter<long> s_userLogoutCounter = s_meter.CreateCounter<long>(Counters.UserLogout);

        /// <summary>
        /// Helper method to increase <see cref="Counters.Consent" /> counter. The scopes
        /// are expanded and called one by one to not cause a combinatory explosion of scopes.
        /// </summary>
        /// <param name="clientId">Client id</param>
        /// <param name="scopes">Scope names. Each element is added on its own to the counter</param>
        /// <param name="remember"></param>
        public static void ConsentGranted(string clientId, IEnumerable<string> scopes, bool remember)
        {
            ArgumentNullException.ThrowIfNull(scopes);

            foreach (string scope in scopes)
                s_consentCounter.Add(1,
                    new KeyValuePair<string, object?>(Tags.Client, clientId),
                    new KeyValuePair<string, object?>(Tags.Scope, scope),
                    new KeyValuePair<string, object?>(Tags.Remember, remember),
                    new KeyValuePair<string, object?>(Tags.Consent, TagValues.Granted));
        }

        /// <summary>
        /// Helper method to increase ConsentDenied counter. The scopes
        /// are expanded and called one by one to not cause a combinatory explosion of scopes.
        /// </summary>
        /// <param name="clientId">Client id</param>
        /// <param name="scopes">Scope names. Each element is added on its own to the counter</param>
        public static void ConsentDenied(string clientId, IEnumerable<string> scopes)
        {
            ArgumentNullException.ThrowIfNull(scopes);
            foreach (string scope in scopes)
                s_consentCounter.Add(1, new KeyValuePair<string, object?>(Tags.Client, clientId),
                    new KeyValuePair<string, object?>(Tags.Scope, scope),
                    new KeyValuePair<string, object?>(Tags.Consent, TagValues.Denied));
        }

        /// <summary>
        /// Helper method to increase the <see cref="Counters.GrantsRevoked" /> counter.
        /// </summary>
        /// <param name="clientId">Client id to revoke for, or null for all.</param>
        public static void GrantsRevoked(string? clientId)
        {
            s_grantsRevokedCounter.Add(1, new KeyValuePair<string, object?>(Tags.Client, clientId));
        }

        /// <summary>
        /// Helper method to increase <see cref="Counters.UserLogin" /> counter.
        /// </summary>
        /// <param name="clientId">Client Id, if available</param>
        /// <param name="idp"></param>
        public static void UserLogin(string? clientId, string idp)
        {
            s_userLoginCounter.Add(1, new KeyValuePair<string, object?>(Tags.Client, clientId),
                new KeyValuePair<string, object?>(Tags.Idp, idp));
        }

        /// <summary>
        /// Helper method to increase <see cref="Counters.UserLogin" /> counter on failure.
        /// </summary>
        /// <param name="clientId">Client Id, if available</param>
        /// <param name="idp"></param>
        /// <param name="error">Error message</param>
        public static void UserLoginFailure(string? clientId, string idp, string error)
        {
            s_userLoginCounter.Add(1, new KeyValuePair<string, object?>(Tags.Client, clientId),
                new KeyValuePair<string, object?>(Tags.Idp, idp),
                new KeyValuePair<string, object?>(Tags.Error, error));
        }

        /// <summary>
        /// Helper method to increase the <see cref="Counters.UserLogout" /> counter.
        /// </summary>
        /// <param name="idp">Idp/authentication scheme for external authentication, or "local" for built-in.</param>
        public static void UserLogout(string? idp)
        {
            s_userLogoutCounter.Add(1, new KeyValuePair<string, object?>(Tags.Idp, idp));
        }
#pragma warning disable 1591

        /// <summary>
        /// Name of Counters
        /// </summary>
        public static class Counters
        {
            public const string Consent = "tokenservice.consent";
            public const string GrantsRevoked = "tokenservice.grants_revoked";
            public const string UserLogin = "tokenservice.user_login";
            public const string UserLogout = "tokenservice.user_logout";
        }

        /// <summary>
        /// Name of tags
        /// </summary>
        public static class Tags
        {
            public const string Client = "client";
            public const string Error = "error";
            public const string Idp = "idp";
            public const string Remember = "remember";
            public const string Scope = "scope";
            public const string Consent = "consent";
        }

        /// <summary>
        /// Values of tags
        /// </summary>
        public static class TagValues
        {
            public const string Granted = "granted";
            public const string Denied = "denied";
        }

#pragma warning restore 1591
    }
}