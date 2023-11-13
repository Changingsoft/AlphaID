namespace IDSubjects.ChineseName;

/// <summary>
/// 读音汉字字符。
/// </summary>
public class PhoneticChineseChar
{
    private string? customPinyin;
    private int selectedIndex;

    /// <summary>
    /// 使用字符和拼音初始化读音汉字字符。
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="pinyins"></param>
    public PhoneticChineseChar(char origin, string[] pinyins)
    {
        this.Origin = origin;
        this.Pinyins = pinyins;
    }

    /// <summary>
    /// 获取标识此汉字的原始字符。
    /// </summary>
    public char Origin { get; }

    /// <summary>
    /// 获取此汉字的发音。
    /// </summary>
    public string[] Pinyins { get; }

    /// <summary>
    /// 获取选择的读音。
    /// </summary>
    public string Selected
    {
        get
        {
            return !string.IsNullOrEmpty(this.customPinyin) ? this.customPinyin : this.Pinyins[this.selectedIndex];
        }
    }

    /// <summary>
    /// 按索引选择一个读音，
    /// </summary>
    /// <param name="index"></param>
    public void Select(int index)
    {
        if (index >= this.Pinyins.Length)
            throw new ArgumentOutOfRangeException(nameof(index));
        this.selectedIndex = index;
    }

    /// <summary>
    /// 选择一个自定义读音。
    /// </summary>
    /// <param name="custom"></param>
    public void Select(string custom)
    {
        if (string.IsNullOrWhiteSpace(custom))
        {
            throw new ArgumentException(Resources.StringIsNullOrWhiteSpace, nameof(custom));
        }

        for (int i = 0; i < this.Pinyins.Length; i++)
        {
            if (this.Pinyins[i].StartsWith(custom))
            {
                this.selectedIndex = i;
                return;
            }
        }

        this.customPinyin = custom.Trim().ToUpper();
    }
}
