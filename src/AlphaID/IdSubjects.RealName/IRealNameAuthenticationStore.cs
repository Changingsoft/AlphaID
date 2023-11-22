using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdSubjects.RealName;

/// <summary>
/// 
/// </summary>
public interface IRealNameAuthenticationStore
{
    /// <summary>
    /// 
    /// </summary>
    IQueryable<RealNameAuthentication> Authentications { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="authentication"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(RealNameAuthentication authentication);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="authentication"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(RealNameAuthentication authentication);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="authentication"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(RealNameAuthentication authentication);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteByPersonIdAsync(string personId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    IQueryable<RealNameAuthentication> FindByPerson(NaturalPerson person);
}
