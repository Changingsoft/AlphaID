using AlphaIdPlatform.Subjects;
using IntegrationTestUtilities;

namespace AlphaIdPlatform.Tests;
public class OrganizationManagerTest(ServiceProviderFixture serviceProviderFixture) : IClassFixture<ServiceProviderFixture>
{
    [Fact]
    public async Task CreateOrganization()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        organizationManager.TimeProvider = new FrozenTimeProvider();
        var organization = new Organization { Name = "Test Organization" };
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
        await organizationManager.CreateAsync(new Organization { Name = "Test Organization" });
        var result = await organizationManager.CreateAsync(new Organization { Name = "Test Organization" });
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task FindOrganizationsViaNameAndUsedName()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        var orgA = new Organization { Name = "OrgA" };
        await organizationManager.CreateAsync(orgA);
        await organizationManager.RenameAsync(orgA, "OrgD", DateOnly.FromDateTime(DateTime.Now));
        var found = organizationManager.FindByName("OrgA");
        Assert.Contains(found, o => o.Name == "OrgD");
        Assert.Contains(orgA.UsedNames, u => u.Name == "OrgA");
    }

    [Fact]
    public async Task RenameOrganizationToCurrentSameName()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        var orgA = new Organization { Name = "OrgA" };
        await organizationManager.CreateAsync(orgA);
        await organizationManager.CreateAsync(new Organization { Name = "OrgB" });
        var result = await organizationManager.RenameAsync(orgA, "OrgB");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task UpdateOrganization()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        //Set time frozen to min value.
        organizationManager.TimeProvider = new FrozenTimeProvider(DateTimeOffset.MinValue);
        var organization = new Organization { Name = "Test Organization" };
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
