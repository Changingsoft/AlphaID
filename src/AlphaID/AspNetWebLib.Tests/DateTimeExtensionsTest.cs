namespace AspNetWebLib.Tests;

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

    [Fact]
    public void AgeCalcWhenBornInLeapYear()
    {
        var dateOfBirth = new DateTime(1988, 2, 29); // Leap year
        var now = new DateTime(2023, 2, 28); // Day before leap day
        Assert.Equal(34, dateOfBirth.AsAge(now));

        var now2 = new DateTime(2023, 3, 1);
        Assert.Equal(35, dateOfBirth.AsAge(now2));

        var now3 = new DateTime(2024, 2, 29); // Next leap year
        Assert.Equal(35, dateOfBirth.AsAge(now3));

        var now4 = new DateTime(2024, 3, 1);
        Assert.Equal(36, dateOfBirth.AsAge(now4));
    }
}