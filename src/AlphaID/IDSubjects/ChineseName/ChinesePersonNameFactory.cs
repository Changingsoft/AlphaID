namespace IDSubjects.ChineseName;

/// <summary>
/// Chinese Person Name Factory.
/// </summary>
public class ChinesePersonNameFactory
{
    private readonly ChinesePersonNamePinyinConverter pinyinConverter;

    /// <summary>
    /// Initialize factory by using default name processor and pinyin converter.
    /// </summary>
    public ChinesePersonNameFactory(ChinesePersonNamePinyinConverter pinyinConverter)
    {
        this.pinyinConverter = pinyinConverter;
    }

    /// <summary>
    /// 初始化默认的中国人姓名工厂。
    /// </summary>
    public ChinesePersonNameFactory() : this(new ChinesePersonNamePinyinConverter())
    { }


    /// <summary>
    /// Create chinese person name from a full name.
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public ChinesePersonName Create(string fullName)
    {
        fullName = fullName.Trim(' ', '\r', '\n').Replace(" ", string.Empty);

        if (fullName.Length <= 1)
            throw new ArgumentException("名字太短");

        string surname = default!;
        foreach (var prefix in CompoundSurnamePrefixes)
        {
            if (fullName.StartsWith(prefix))
            {
                surname = prefix;
                fullName = fullName.Remove(0, prefix.Length);
                break;
            }
        }
        if (surname == null)
        {
            surname = fullName[..1];
            fullName = fullName[1..];
        }

        if (fullName.Length < 1)
            throw new ArgumentException("名字太短");

        string givenName = fullName;
        var (phoneticSurname, phoneticGivenName) = this.pinyinConverter.Convert(surname, givenName);
        return new ChinesePersonName(surname, givenName, phoneticSurname, phoneticGivenName);

    }

    private static readonly List<string> CompoundSurnamePrefixes = new()
    {
        "欧阳",
        "太史",
        "端木",
        "上官",
        "司马",
        "东方",
        "独孤",
        "南宫",
        "万俟",
        "闻人",
        "夏侯",
        "诸葛",
        "尉迟",
        "公羊",
        "赫连",
        "澹台",
        "皇甫",
        "宗政",
        "濮阳",
        "公冶",
        "太叔",
        "申屠",
        "公孙",
        "慕容",
        "仲孙",
        "钟离",
        "长孙",
        "宇文",
        "司徒",
        "鲜于",
        "司空",
        "闾丘",
        "子车",
        "亓官",
        "司寇",
        "巫马",
        "公西",
        "颛孙",
        "壤驷",
        "公良",
        "漆雕",
        "乐正",
        "宰父",
        "谷梁",
        "拓跋",
        "夹谷",
        "轩辕",
        "令狐",
        "段干",
        "百里",
        "呼延",
        "东郭",
        "南门",
        "羊舌",
        "微生",
        "公户",
        "公玉",
        "公仪",
        "梁丘",
        "公仲",
        "公上",
        "公门",
        "公山",
        "公坚",
        "左丘",
        "公伯",
        "西门",
        "公祖",
        "第五",
        "公乘",
        "贯丘",
        "公皙",
        "南荣",
        "东里",
        "东宫",
        "仲长",
        "子书",
        "子桑",
        "即墨",
        "达奚",
        "褚师",
        "吴铭"
    };

}
