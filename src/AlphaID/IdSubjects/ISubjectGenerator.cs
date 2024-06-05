using System.Security.Claims;

namespace IdSubjects;

/// <summary>
/// </summary>
public interface ISubjectGenerator
{
    /// <summary>
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    string Generate(ClaimsPrincipal principal);
}