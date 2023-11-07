using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IDSubjects.Tests;
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
