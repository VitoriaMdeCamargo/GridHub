using GridHub.API.Configuration;
using GridHub.Database.Models;
using GridHub.Repository.Interface;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace GridHub.API.Extensions
{
    public static class ServiceCollectionsExtensions
    {
       
        public static IServiceCollection AddSwagger(this IServiceCollection services, APPConfiguration configuration)
        {
            services.AddSwaggerGen(swagger =>
            {
                // Adiciona a possibilidade de enviar token para o controller
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                // Configura a documentação do Swagger
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = configuration.Swagger.Title,
                    Description = configuration.Swagger.Description,
                    Contact = new OpenApiContact()
                    {
                        Email = configuration.Swagger.Email,
                        Name = configuration.Swagger.Name
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);
            });



            return services;
        }

        public static IServiceCollection AddHealthCheck(this IServiceCollection services, APPConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddOracle(configuration.OracleFIAP.Connection, name: configuration.OracleFIAP.Name)
                .AddUrlGroup(new Uri("https://viacep.com.br/"), name: "VIA CEP");

            return services;
        }
    }
}
