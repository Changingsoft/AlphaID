namespace RadiusCore.Dictionary;

/// <summary>
/// Create a dictionary rfc attribute
/// </summary>
/// <param name="name"></param>
/// <param name="code"></param>
/// <param name="type"></param>
public class DictionaryAttribute(string name, byte code, string type)
{
    /// <summary>
    /// 
    /// </summary>
    public readonly byte Code = code;
    /// <summary>
    /// 
    /// </summary>
    public readonly string Name = name;
    /// <summary>
    /// 
    /// </summary>
    public readonly string Type = type;
}