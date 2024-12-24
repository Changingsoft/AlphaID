namespace IdSubjects.ChineseName;

/// <summary>
///     读音汉字字符。
/// </summary>
/// <remarks>
///     使用字符和拼音初始化读音汉字字符。
/// </remarks>
/// <param name="origin"></param>
/// <param name="pinyins"></param>
public class PhoneticChineseChar(char origin, string[] pinyins)
{
    private string? _customPinyin;
    private int _selectedIndex;

    /// <summary>
    ///     获取标识此汉字的原始字符。
    /// </summary>
    public char Origin { get; } = origin;

    /// <summary>
    ///     获取此汉字的发音。
    /// </summary>
    public string[] Pinyins { get; } = pinyins;

    /// <summary>
    ///     获取选择的读音。
    /// </summary>
    public string Selected => !string.IsNullOrEmpty(_customPinyin) ? _customPinyin : Pinyins[_selectedIndex];

    /// <summary>
    ///     按索引选择一个读音，
    /// </summary>
    /// <param name="index"></param>
    public void Select(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Pinyins.Length);
        _selectedIndex = index;
    }

    /// <summary>
    ///     选择一个自定义读音。
    /// </summary>
    /// <param name="custom"></param>
    public void Select(string custom)
    {
        if (string.IsNullOrWhiteSpace(custom))
            throw new ArgumentException(Resources.StringIsNullOrWhiteSpace, nameof(custom));

        for (var i = 0; i < Pinyins.Length; i++)
            if (Pinyins[i].StartsWith(custom))
            {
                _selectedIndex = i;
                return;
            }

        _customPinyin = custom.Trim().ToUpper();
    }
}