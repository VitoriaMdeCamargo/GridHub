using GridHub.API.Configuration;
using GridHub.Repository.Interface;
using GridHub.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GridHub.Database;
using GridHub.Database.Models;
using Microsoft.Extensions.Options;
using System.Reflection;
using ERP_InsightWise.Service.CEP;
using Microsoft.Extensions.DependencyInjection;
using ERP_InsightWise.API.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using GridHub.API.Extensions;
using Stripe;

namespace GridHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IConfiguration configuration = builder.Configuration;

            APPConfiguration appConfiguration = new APPConfiguration();

            builder.Services.Configure<APPConfiguration>(configuration);

            configuration.Bind(appConfiguration);

            StripeConfiguration.ApiKey = configuration["Stripe:ApiKey"];


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(swagger =>
            {
                // Carregar o arquivo XML de comentários
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);

                //Adiciona a possibilidade de enviar token para o controller
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

                //Codigo para mudar a documentação do Swagger
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = builder.Configuration.GetSection("Swagger:Title").Value,
                    Description = builder.Configuration.GetSection("Swagger:Description").Value,
                    Contact = new OpenApiContact()
                    {
                        Email = builder.Configuration.GetSection("Swagger:Email").Value,
                        Name = builder.Configuration.GetSection("Swagger:Name").Value
                    }
                });
            });

            builder.Services.AddDbContext<FIAPDBContext>(options =>
            {
                options.UseOracle(builder.Configuration.GetConnectionString("FIAPDatabase"),
                    b => b.MigrationsAssembly("GridHub.Database"));
            });

            builder.Services.AddScoped<IRepository<Usuario>, Repository<Usuario>>();
            builder.Services.AddScoped<IRepository<Espaco>, Repository<Espaco>>();
            builder.Services.AddScoped<IRepository<Microgrid>, Repository<Microgrid>>();
            builder.Services.AddScoped<IRepository<Investimento>, Repository<Investimento>>();
            builder.Services.AddScoped<IRepository<Relatorio>, Repository<Relatorio>>();
            builder.Services.AddScoped<ICEPService, CEPService>();
            builder.Services.AddHealthCheck(appConfiguration);
            
            builder.Services.AddLogging(builder =>
            {
                builder.AddConsole();  
                builder.SetMinimumLevel(LogLevel.Debug);  
            });
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowAllOrigins");

            app.MapControllers();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health-check", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = HealthCheckExtensions.WriteResponse
                });
            });

            app.Run();
        }
    }
}
