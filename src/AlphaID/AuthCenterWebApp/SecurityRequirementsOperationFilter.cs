using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthCenterWebApp;

internal class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        List<AuthorizeAttribute> authAttributes = context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>()
            .ToList();

        if (authAttributes.Count != 0)
        {
            //operation.Responses.Add(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse { Description = nameof(HttpStatusCode.Unauthorized) });
            //operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse { Description = nameof(HttpStatusCode.Forbidden) });
        }

        if (authAttributes.Count != 0)
        {
            operation.Security = [];

            var oauth2SecurityScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" }
            };

            List<string> scopes =
            [
                "openid",
                "profile",
                "membership"
            ];

            if (authAttributes.Any(p => p.Policy == "RealNameScopeRequired"))
                scopes.Add("realname");

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [oauth2SecurityScheme] = scopes
            });
        }
    }
}