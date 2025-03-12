namespace RadiusCore.Dictionary
{
    /// <summary>
    /// Create a dictionary rfc attribute
    /// </summary>
    /// <param name="name"></param>
    /// <param name="code"></param>
    /// <param name="type"></param>
    public class DictionaryAttribute(string name, byte code, string type)
    {
        public readonly byte Code = code;
        public readonly string Name = name;
        public readonly string Type = type;
    }
}
