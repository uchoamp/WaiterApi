using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Waiter.API.Custom
{
    class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize = context
                .ApiDescription.CustomAttributes()
                .Any(attr => attr.GetType() == typeof(AuthorizeAttribute));

            var hasAnnomymous = context
                .ApiDescription.CustomAttributes()
                .Any(attr => attr.GetType() == typeof(AllowAnonymousAttribute));

            if (hasAuthorize && !hasAnnomymous)
            {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            },
                            new List<string>()
                        }
                    }
                };

                operation.Responses.Add(
                    "401",
                    new OpenApiResponse
                    {
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            {
                                "application/json",
                                new OpenApiMediaType
                                {
                                    Example = new OpenApiObject
                                    {
                                        {
                                            "message",
                                            new OpenApiString(
                                                "Bearer token required or user not authorized"
                                            )
                                        }
                                    }
                                }
                            }
                        },

                        Description = "Bearer token required or user not authorized"
                    }
                );
            }
        }
    }
}
