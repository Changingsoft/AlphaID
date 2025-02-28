namespace IdSubjects.ChineseName;

/// <summary>
/// 名字拼音转换上下文。
/// </summary>
public class ChineseNamePinyinConvertContext
{
    internal ChineseNamePinyinConvertContext()
    {
        SurnameChars = [];
        GivenNameChars = [];
    }

    internal ChineseNamePinyinConvertContext(IEnumerable<PhoneticChineseChar> surnameChars,
        IEnumerable<PhoneticChineseChar> givenNameChars)
    {
        SurnameChars = new List<PhoneticChineseChar>(surnameChars);
        GivenNameChars = new List<PhoneticChineseChar>(givenNameChars);
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