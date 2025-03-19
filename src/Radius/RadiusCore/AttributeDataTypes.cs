namespace RadiusCore
{
    /// <summary>
    /// 属性值的格式。
    /// </summary>
    public enum AttributeValueFormat
    {
        /// <summary>
        /// UTF-8 编码的文本。
        /// </summary>
        Text,
        /// <summary>
        /// 二进制数据。以字节为单位。
        /// </summary>
        String,
        /// <summary>
        /// 32位地址值，以网络序存储。
        /// </summary>
        Address,
        /// <summary>
        /// 32位无符号整数，以网络序存储。
        /// </summary>
        Integer,
        /// <summary>
        /// 32位无符号整数，以UnixTimeSeconds存储。
        /// </summary>
        Time,
    }
}
