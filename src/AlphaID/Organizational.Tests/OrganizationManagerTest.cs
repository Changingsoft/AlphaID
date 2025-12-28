using IntegrationTestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Organizational.Tests;
public class OrganizationManagerTest(ServiceProviderFixture serviceProviderFixture) : IClassFixture<ServiceProviderFixture>
{
    [Fact]
    public async Task CreateOrganization()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        organizationManager.TimeProvider = new FrozenTimeProvider();
        var organization = new Organization("Test Organization");
        var result = await organizationManager.CreateAsync(organization);
        Assert.True(result.Succeeded);
        Assert.Equal(organizationManager.TimeProvider.GetUtcNow(), organization.WhenCreated);
        Assert.Equal(organizationManager.TimeProvider.GetUtcNow(), organization.WhenChanged);
    }

    [Fact]
    public async Task CreateAnotherOrganizationWithSameName()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        await organizationManager.CreateAsync(new Organization ("Test Organization"));
        var result = await organizationManager.CreateAsync(new Organization ("Test Organization"));
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task RenameOrganizationToCurrentSameName()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        var orgA = new Organization ("OrgA");
        await organizationManager.CreateAsync(orgA);
        await organizationManager.CreateAsync(new Organization ("OrgB"));
        var result = await organizationManager.ChangeName(orgA.Id, "OrgB");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task UpdateOrganization()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        //Set time frozen to min value.
        organizationManager.TimeProvider = new FrozenTimeProvider(DateTimeOffset.MinValue);
        var organization = new Organization ("Test Organization");
        await organizationManager.CreateAsync(organization);

        //Set time frozen to now.
        organizationManager.TimeProvider = new FrozenTimeProvider();
        organization.Domicile = "Test Domicile";
        var result = await organizationManager.UpdateAsync(organization);
        Assert.True(result.Succeeded);
        Assert.Equal(DateTimeOffset.MinValue, organization.WhenCreated);
        Assert.Equal(organizationManager.TimeProvider.GetUtcNow(), organization.WhenChanged);
    }
}
