using System.ComponentModel.DataAnnotations;

namespace IdSubjects;

/// <summary>
///     表示社会性别。
/// </summary>
public enum Gender
{
    /// <summary>
    ///     男
    /// </summary>
    [Display(Name = "Male", ResourceType = typeof(Resources))]
    Male = 1,

    /// <summary>
    ///      女
    /// </summary>
    [Display(Name = "Female", ResourceType = typeof(Resources))]
    Female = 0,

    /// <summary>
    ///     其他
    /// </summary>
    [Display(Name = "Other", ResourceType = typeof(Resources))]
    Other = 2
}