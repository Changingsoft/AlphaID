using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NgAdminWebApp.Models;

namespace NgAdminWebApp.Data;

public class ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
    : ApiAuthorizationDbContext<ApplicationUser>(options, operationalStoreOptions);