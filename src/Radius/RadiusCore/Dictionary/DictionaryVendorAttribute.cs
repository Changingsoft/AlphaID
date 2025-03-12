namespace RadiusCore.Dictionary
{
    /// <summary>
    /// Create a dictionary vendor specific attribute
    /// </summary>
    /// <param name="vendorId"></param>
    /// <param name="name"></param>
    /// <param name="vendorCode"></param>        
    /// <param name="type"></param>
    public class DictionaryVendorAttribute(uint vendorId, string name, uint vendorCode, string type) : DictionaryAttribute(name, 26, type)
    {
        public readonly uint VendorId = vendorId;
        public readonly uint VendorCode = vendorCode;
    }
}
