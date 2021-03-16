using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Smairo.AspNetHosting.Filters;

namespace Smairo.AspNetHosting
{
    /// <summary>
    /// Basic startup that adds controllers, json and swagger
    /// and allows you to create clean and structured startup
    /// </summary>
    public abstract class ApiStartup
    {
        /// <summary>
        /// Create swagger info for the api
        /// </summary>
        public abstract OpenApiInfo ApiInfo { get; }

        /// <summary>
        /// Configuration from configuration sources
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// Environment where api is running
        /// </summary>
        protected IWebHostEnvironment Environment { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        protected ApiStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        /// Aspnet spec, runs when host starting
        /// </summary>
        /// <param name="services"></param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            AddBuiltInServices(services);
            AddSwagger(services, ApiInfo);
            AddAndConfigureOptions(services);
            AddAuthenticationAndAuthorization(services);
            AddDatabase(services);
            AddOurServices(services);
        }

        /// <summary>
        /// Aspnet spec, runs when host is finishing startup
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<ApiStartup> logger)
        {
            app.UseRouting();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.RoutePrefix = string.Empty;
                opt.DisplayRequestDuration();
                opt.EnableFilter();
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                opt.InjectStylesheet("css/swagger/custom-theme.css");
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            logger.LogInformation("Api startup is now done. " +
                $"Environment for the api is: {env.EnvironmentName}");
        }

        /// <summary>
        /// Adds controllers, json, compat version, logging, http accessor and http client factory
        /// </summary>
        /// <param name="services"></param>
        public virtual void AddBuiltInServices(IServiceCollection services)
        {
            services
                .AddControllers(opt =>
                {
                    opt.Filters.Add(new ProducesAttribute("application/json"));
                    opt.Filters.Add(new ConsumesAttribute("application/json"));
                    opt.Filters.Add(new ProducesResponseTypeAttribute((int) HttpStatusCode.InternalServerError));
                })
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.WriteIndented = true;
                    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services
                .AddLogging()
                .AddHttpContextAccessor()
                .AddHttpClient();
        }

        /// <summary>
        /// Adds swagger with bearer security scheme
        /// </summary>
        /// <param name="services"></param>
        /// <param name="info"></param>
        public virtual void AddSwagger(IServiceCollection services, OpenApiInfo info)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", info);
                c.IncludeXmlComments(ApiDocumentationXmlPath);
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
                c.DocumentFilter<HideAdditionalPropertiesFilter>();
                c.UseAllOfToExtendReferenceSchemas();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        System.Array.Empty<string>()
                    },
                });
            });
        }

        /// <summary>
        /// Here you should do all configurations for <see cref="IOptions{TOptions}"/>
        /// </summary>
        /// <param name="services"></param>
        public abstract void AddAndConfigureOptions(IServiceCollection services);

        /// <summary>
        /// Here you should add authentication methods and authorization policies
        /// </summary>
        /// <param name="services"></param>
        public abstract void AddAuthenticationAndAuthorization(IServiceCollection services);

        /// <summary>
        /// Here you should add your model and repositories
        /// </summary>
        /// <param name="services"></param>
        public abstract void AddDatabase(IServiceCollection services);

        /// <summary>
        /// Here you should add all services and other you are using within api
        /// </summary>
        /// <param name="services"></param>
        public abstract void AddOurServices(IServiceCollection services);

        /// <summary>
        /// Gets doc path from generated documentation xml file for api.
        /// </summary>
        /// <returns></returns>
        public abstract string ApiDocumentationXmlPath { get; }

    }
}
