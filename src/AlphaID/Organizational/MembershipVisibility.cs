namespace Organizational;

/// <summary>
/// Membership Visibility
/// </summary>
public enum MembershipVisibility
{
    /// <summary>
    /// The membership is only visible to other members of this organization.
    /// </summary>
    Private = 0,

    /// <summary>
    /// The membership is visible to users who where authenticated by Alpha ID.
    /// </summary>
    AuthenticatedUser = 6,

    /// <summary>
    /// The membership is visible to everyone and is displayed on your public profile.
    /// </summary>
    Public = 7
}