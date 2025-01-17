using IdSubjects.ChineseName;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace IdSubjects.Tests;

public class ApplicationUserBuilderTests
{
    [Fact]
    public void UseChinesePersonNameTest()
    {
        var builder = new ApplicationUserBuilder<ApplicationUser>("HumanName", new HumanNameInfo("FullName", "Surname", "GivenName"));
        builder.UseChinesePersonName(new ChinesePersonName("张", "三", "ZHANG", "SAN"));

        ApplicationUser person = builder.Build();
        Assert.Equal("三", person.HumanName?.GivenName);
        Assert.Equal("张三", person.HumanName?.FullName);
        //Assert.Equal("ZHANG", person.PhoneticSurname);
        //Assert.Equal("SAN", person.PhoneticGivenName);
    }
}