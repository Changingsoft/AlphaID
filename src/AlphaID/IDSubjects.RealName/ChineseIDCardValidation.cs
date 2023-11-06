using IDSubjects.ChineseName;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects.RealName;

/// <summary>
/// 实名验证信息。
/// </summary>
[Table("RealNameValidation")]
public class ChineseIdCardValidation
{
    /// <summary>
    /// 
    /// </summary>
    protected ChineseIdCardValidation() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chineseIDCardImage"></param>
    public ChineseIdCardValidation(ChineseIDCardImage chineseIDCardImage)
    {
        this.ChineseIDCardImage = chineseIDCardImage;
    }


    /// <summary>
    /// Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected internal set; }

    /// <summary>
    /// 关联的自然人。
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string PersonId { get; protected internal set; } = default!;

    /// <summary>
    /// 与此关联的自然人。
    /// </summary>
    [ForeignKey(nameof(PersonId))]
    public virtual NaturalPerson Person { get; protected internal set; } = default!;

    /// <summary>
    /// 身份证信息
    /// </summary>
    public virtual ChineseIDCardInfo? ChineseIDCard { get; set; }

    /// <summary>
    /// 身份证扫描件
    /// </summary>
    public virtual ChineseIDCardImage? ChineseIDCardImage { get; protected set; }

    /// <summary>
    /// 姓名。
    /// </summary>
    public virtual ChinesePersonName? ChinesePersonName { get; set; }

    /// <summary>
    /// 提交时间。
    /// </summary>
    public DateTime CommitTime { get; protected internal set; } = default!;

    /// <summary>
    /// 认证结果。
    /// </summary>
    public virtual ValidationResult? Result { get; protected internal set; }

    /// <summary>
    /// 尝试添加身份证信息。
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool TryApplyChineseIdCardInfo(ChineseIDCardInfo card)
    {
        if (this.Result != null) return false;
        this.ChineseIDCard = card;
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chinesePersonName"></param>
    /// <returns></returns>
    public bool TryApplyChinesePersonName(ChinesePersonName chinesePersonName)
    {
        if (this.Result != null)
            return false;
        this.ChinesePersonName = chinesePersonName;
        return true;
    }
}
