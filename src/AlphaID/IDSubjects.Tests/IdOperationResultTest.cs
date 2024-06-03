using Xunit;

namespace IdSubjects.Tests;

public class IdOperationResultTest
{
    [Fact]
    public void FailedWithNoErrorMessage()
    {
        IdOperationResult result = IdOperationResult.Failed();
        Assert.False(result.Succeeded);
        Assert.False(result.Errors.Any());
    }
}