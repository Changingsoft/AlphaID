using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;

namespace AlphaIDWebAPI;

internal class SecurityRequirementsOperationFilter : IOperationFilter
{

    public SecurityRequirementsOperationFilter()
    { }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authAttributes = context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
                            .Union(context.MethodInfo.GetCustomAttributes(true))
                            .OfType<AuthorizeAttribute>();

        if (authAttributes.Any())
        {
            operation.Responses.Add(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse { Description = nameof(HttpStatusCode.Unauthorized) });
            operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse { Description = nameof(HttpStatusCode.Forbidden) });
        }

        if (authAttributes.Any())
        {
            operation.Security = new List<OpenApiSecurityRequirement>();

            var oauth2SecurityScheme = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" },
            };

            List<string> scopes = new()
            {
                "openid"
            };

            if (authAttributes.Any(p => p.Policy == "RealNameScopeRequired"))
                scopes.Add("realname");

            operation.Security.Add(new OpenApiSecurityRequirement()
            {
                [oauth2SecurityScheme] = scopes,
            });
        }
    }
}