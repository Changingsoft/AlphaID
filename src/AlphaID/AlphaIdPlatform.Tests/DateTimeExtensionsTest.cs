namespace AlphaIdPlatform.Tests;

public class DateTimeExtensionsTest
{
    [Fact]
    public void LessYearlingAge()
    {
        var dateOfBirth = new DateTime(1985, 4, 20);
        var now = new DateTime(2023, 4, 20);
        Assert.Equal(37, dateOfBirth.AsAge(now));
    }

    [Fact]
    public void FullYearlingAge()
    {
        var dateOfBirth = new DateTime(1985, 4, 20);
        var now = new DateTime(2023, 4, 21);
        Assert.Equal(38, dateOfBirth.AsAge(now));
    }
}