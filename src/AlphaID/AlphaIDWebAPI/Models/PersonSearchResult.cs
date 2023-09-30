namespace AlphaIDWebAPI.Models;

/// <summary>
/// 自然人搜索结果。
/// </summary>
/// <param name="Persons">自然人搜索的结果。</param>
/// <param name="More">指示一个值，表示该结果不完全，需要尝试更多的关键字来缩小搜索范围。</param>
public record PersonSearchResult(IEnumerable<PersonModel> Persons, bool More = false);
