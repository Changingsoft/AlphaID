using AlphaIdPlatform.Subjects;

namespace AlphaIdPlatform.Tests;
public class OrganizationManagerTest(ServiceProviderFixture serviceProviderFixture) : IClassFixture<ServiceProviderFixture>
{
    [Fact]
    public async Task CreateOrganization()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        var organization = new Organization
        {
            Name = "Test Organization",
            Description = "Test Description"
        };
        var result = await organizationManager.CreateAsync(organization);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task CreateAnotherOrganizationWithSameName()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        var organization = new Organization
        {
            Name = "Test Organization",
            Description = "Test Description"
        };
        _ = await organizationManager.CreateAsync(organization);
        var anotherOrganization = new Organization
        {
            Name = "Test Organization",
            Description = "Another Test Description"
        };
        var result = await organizationManager.CreateAsync(anotherOrganization);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task FindOrganizationsViaNameAndUsedName()
    {
        using var scope = serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var organizationManager = scope.ServiceProvider.GetRequiredService<OrganizationManager>();
        var orgA = new Organization { Name = "OrgA" };
        await organizationManager.CreateAsync(orgA);
        await organizationManager.ChangeNameAsync(orgA, "OrgD", DateOnly.FromDateTime(DateTime.Now));
        var found = organizationManager.FindByName("OrgA");
        Assert.Contains(found, o => o.Name == "OrgD");
        Assert.Contains(orgA.UsedNames, u => u.Name == "OrgA");
    }
}
