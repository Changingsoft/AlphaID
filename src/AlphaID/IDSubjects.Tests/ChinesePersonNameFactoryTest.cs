using IdSubjects.ChineseName;
using Xunit;

namespace IdSubjects.Tests;

public class ChinesePersonNameFactoryTest
{
    [Fact]
    public void SplitSurnameAndGivenNameTest()
    {
        var factory = new ChinesePersonNameFactory();
        var name1 = factory.Create("张三");
        Assert.Equal("张", name1.Surname);
        Assert.Equal("三", name1.GivenName);

        var name2 = factory.Create("张晓丽");
        Assert.Equal("张", name2.Surname);
        Assert.Equal("晓丽", name2.GivenName);

        var name3 = factory.Create("张晓丽娜");
        Assert.Equal("张", name3.Surname);
        Assert.Equal("晓丽娜", name3.GivenName);

        var name4 = factory.Create("欧阳修");
        Assert.Equal("欧阳", name4.Surname);
        Assert.Equal("修", name4.GivenName);

        var name5 = factory.Create("司马长空");
        Assert.Equal("司马", name5.Surname);
        Assert.Equal("长空", name5.GivenName);
    }

    /// <summary>
    /// 名字太短
    /// </summary>
    [Fact]
    public void FullNameTooShortTest()
    {
        var factory = new ChinesePersonNameFactory();
        Assert.Throws<ArgumentException>(() => factory.Create("张"));
        Assert.Throws<ArgumentException>(() => factory.Create("欧阳"));
    }
}