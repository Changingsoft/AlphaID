using Xunit;

namespace IdSubjects.Tests;
public class IdOperationResultTest
{
    [Fact]
    public void FailedWithNoErrorMessage()
    {
        var result = IdOperationResult.Failed();
        Assert.False(result.Succeeded);
        Assert.False(result.Errors.Any());
    }
}
