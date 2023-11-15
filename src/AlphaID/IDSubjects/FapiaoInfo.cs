using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IdSubjects;

/// <summary>
/// 
/// </summary>
[Owned]
public class FapiaoInfo
{
    /// <summary>
    /// 
    /// </summary>
    [MaxLength(30)] public string Name { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(30)]
    public string TaxPayerId { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(30)]
    public string Address { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(30)]
    public string Contact { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(30)]
    public string Bank { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(30)]
    public string Account { get; set; } = default!;
}
