using IdSubjects.ChineseName;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace IdSubjects.Tests;

public class ApplicationUserBuilderTests
{
    [Fact]
    public void UseChinesePersonNameTest()
    {
        var builder = new ApplicationUserBuilder<ApplicationUser>("PersonName", new PersonNameInfo("FullName", "Surname", "GivenName"));
        builder.UseChinesePersonName(new ChinesePersonName("张", "三", "ZHANG", "SAN"));

        ApplicationUser person = builder.Build();
        Assert.Equal("三", person.PersonName.GivenName);
        Assert.Equal("张三", person.PersonName.FullName);
        //Assert.Equal("ZHANG", person.PhoneticSurname);
        //Assert.Equal("SAN", person.PhoneticGivenName);
    }
}