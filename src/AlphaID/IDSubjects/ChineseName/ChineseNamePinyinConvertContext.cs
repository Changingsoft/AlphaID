namespace IDSubjects.ChineseName;

/// <summary>
/// 名字拼音转换上下文。
/// </summary>
public class ChineseNamePinyinConvertContext
{
    internal ChineseNamePinyinConvertContext()
    {
        this.SurnameChars = new List<PhoneticChineseChar>();
        this.GivenNameChars = new List<PhoneticChineseChar>();
    }

    internal ChineseNamePinyinConvertContext(IEnumerable<PhoneticChineseChar> surnameChars, IEnumerable<PhoneticChineseChar> givenNameChars)
    {
        this.SurnameChars = new List<PhoneticChineseChar>(surnameChars);
        this.GivenNameChars = new List<PhoneticChineseChar>(givenNameChars);
    }

    /// <summary>
    /// 组成姓氏的汉字读音字符列表。
    /// </summary>
    public List<PhoneticChineseChar> SurnameChars { get; }

    /// <summary>
    /// 组成名字的汉字读音字符列表。
    /// </summary>
    public List<PhoneticChineseChar> GivenNameChars { get; }
}
