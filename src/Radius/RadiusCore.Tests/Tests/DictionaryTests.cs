using RadiusCore.Dictionary;
using Xunit;

namespace RadiusCore.Tests.Tests;

public class DictionaryTests
{
    [Fact]
    public void TestLastItemPrevails()
    {
        const string dictionaryString =
            """
            Attribute	1	User-Name	string
            Attribute	2	User-Password	octet
            Attribute	3	User-Name	octet

            VendorSpecificAttribute	5	3	Acc-Input-Errors	integer
            VendorSpecificAttribute	5	4	Acc-Input-Errors	octet
            """;

        var dictionary = RadiusDictionary.Parse(dictionaryString);

        Assert.Multiple(() =>
        {
            var attributeByName = dictionary.GetAttribute("User-Name")!;
            Assert.Equal(3, attributeByName.Code);
            Assert.Equal("octet", attributeByName.Type);

            var attributeByCode = dictionary.GetAttribute(3);
            Assert.Equal("User-Name", attributeByCode!.Name);

            var vendorAttributeByName = dictionary.GetAttribute("User-Name")!;
            Assert.Equal(3, vendorAttributeByName.Code);
            Assert.Equal("octet", vendorAttributeByName.Type);

            var vendorAttributeByCode = dictionary.GetVendorAttribute(5, 4)!;
            Assert.Equal("Acc-Input-Errors", vendorAttributeByCode.Name);
            Assert.Equal("octet", vendorAttributeByCode.Type);
        });
    }
}