using IdSubjects.RealName.Requesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IdSubjects.RealName.Tests;
public class RealNameRequestManagerTest : IClassFixture<ServiceProviderFixture>
{
    private ServiceProviderFixture serviceProvider;

    public RealNameRequestManagerTest(ServiceProviderFixture serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    [Fact]
    public async Task AddRequest()
    {
        var request = new ChineseIdCardRealNameRequest("张三", Sex.Male, "汉", new DateOnly(1990, 1, 1), "Address",
            "370686193704095897", "Issuer", new DateOnly(2000, 1, 1), new DateOnly(2020, 1, 1),
            new BinaryDataInfo("image/jpg", new byte[] { 0xff, 0xfe }),
            new BinaryDataInfo("image/jpg", new byte[] { 0xff, 0xfe }));
        var person = new NaturalPerson("zhangsan", new PersonNameInfo("张三"));

        using var scope = this.serviceProvider.ScopeFactory.CreateScope();
        {
            var manager = scope.ServiceProvider.GetRequiredService<RealNameRequestManager>();
            var result  = await manager.CreateAsync(person, request);

            Assert.True(result.Succeeded);
            Assert.Equal(person.Id, request.PersonId);
            Assert.False(request.Accepted.HasValue);
        }
    }
}
