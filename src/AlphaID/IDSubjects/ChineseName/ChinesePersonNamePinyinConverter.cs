using Microsoft.International.Converters.PinYinConverter;
using System.Text;

namespace IDSubjects.ChineseName;

/// <summary>
/// 中国人名字拼音转换器。
/// </summary>
public class ChinesePersonNamePinyinConverter
{
    /// <summary>
    /// 初始化转换器。
    /// </summary>
    /// <param name="nameCharactorSpacing">设置一个值，指示名称汉字间是否有空格。</param>
    /// <param name="withTone">设置一个值，指示获取的汉字读音是否包含音调。</param>
    /// <param name="interceptors">设置转换拦截器集合。</param>
    public ChinesePersonNamePinyinConverter(bool nameCharactorSpacing, bool withTone, ICollection<IChinesePersonNamePinyinInterceptor> interceptors)
    {
        this.NameCharactorSpacing = nameCharactorSpacing;
        this.WithTone = withTone;
        this.Interceptors = interceptors;
    }

    /// <summary>
    /// 初始化拼音转换器。
    /// 使用默认的拼音拦截器。
    /// </summary>
    public ChinesePersonNamePinyinConverter() : this(false, false, new IChinesePersonNamePinyinInterceptor[] { new DefaultChinesePersonNamePinyinInterceptor() })
    {

    }

    /// <summary>
    /// 获取或设置一个值，用来表示汉字读音之间是否需要空格。
    /// </summary>
    public bool NameCharactorSpacing { get; set; }

    /// <summary>
    /// 获取或设置一个值，指示拼音是否包括音调。
    /// </summary>
    public bool WithTone { get; set; }

    /// <summary>
    /// 获取转换时用来处理拼音的拦截器集合。
    /// </summary>
    public ICollection<IChinesePersonNamePinyinInterceptor> Interceptors { get; }

    /// <summary>
    /// 将一个汉字转换成读音汉字。
    /// </summary>
    /// <param name="chineseChar">要转换的汉字。</param>
    /// <returns></returns>
    public PhoneticChineseChar Convert(char chineseChar)
    {
        if (!ChineseChar.IsValidChar(chineseChar))
        {
            return new PhoneticChineseChar(chineseChar, new[] { $"{char.ToUpper(chineseChar)}" });
        }

        var cc = new ChineseChar(chineseChar);
        var pinyinList = new List<string>();
        for (int i = 0; i < cc.PinyinCount; i++)
        {
            pinyinList.Add(!this.WithTone ? cc.Pinyins[i].ToUpper()[..^1] : cc.Pinyins[i].ToUpper());
        }
        return new PhoneticChineseChar(chineseChar, pinyinList.ToArray());

    }

    /// <summary>
    /// 将中国人姓名转换为拼音。
    /// </summary>
    /// <returns></returns>
    public (string phoneticSurname, string phoneticGivenName) Convert(string surname, string givenName)
    {
        var context = new ChineseNamePinyinConvertContext();

        if (!string.IsNullOrEmpty(surname))
        {
            foreach (var surnameChar in surname)
            {
                context.SurnameChars.Add(this.Convert(surnameChar));
            }
        }


        foreach (var givenNameChar in givenName)
        {
            context.GivenNameChars.Add(this.Convert(givenNameChar));
        }

        foreach (var interceptor in this.Interceptors)
        {
            interceptor.AfterConvert(context);
        }

        //Build output.
        var surnameBuilder = new StringBuilder();
        foreach (var surnameChar in context.SurnameChars)
        {
            if (this.NameCharactorSpacing)
                surnameBuilder.Append($" {surnameChar.Selected}");
            else
                surnameBuilder.Append(surnameChar.Selected);
        }

        var givenNameBuilder = new StringBuilder();
        foreach (var givenNameChar in context.GivenNameChars)
        {
            if (this.NameCharactorSpacing)
                givenNameBuilder.Append($" {givenNameChar.Selected}");
            else
                givenNameBuilder.Append(givenNameChar.Selected);
        }

        return new(surnameBuilder.ToString().Trim(), givenNameBuilder.ToString().Trim());
    }
}
