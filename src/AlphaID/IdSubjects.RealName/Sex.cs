using System.ComponentModel.DataAnnotations;

namespace IdSubjects.RealName;

/// <summary>
///     生理性别。
/// </summary>
public enum Sex
{
    /// <summary>
    ///     男。
    /// </summary>
    [Display(Name = "Male")]
    Male = 1,

    /// <summary>
    ///     女。
    /// </summary>
    [Display(Name = "Female")]
    Female = 0
}