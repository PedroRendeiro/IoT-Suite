using IoTSuite.Server.Controllers;
using IoTSuite.Server.Filters;
using IoTSuite.Server.Helpers;
using IoTSuite.Server.Hubs;
using IoTSuite.Server.Models;
using IoTSuite.Server.Services;
using IoTSuite.Server.Tasks;
using IoTSuite.Shared;
using IoTSuite.Shared.Wrappers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IoTSuite.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Bearer", jwtOptions =>
            {
                jwtOptions.Authority = Configuration["AzureAdB2C:Authority"];
                jwtOptions.Audience = Configuration["AzureAdB2C:ClientId"];
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };

                // Sending the access token in the query string is required due to
                // a limitation in Browser APIs. We restrict it to only calls to the
                // SignalR hub in this code.
                // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
                // for more information about security considerations when using
                // the query string to transmit the access token.
                jwtOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/SignalR")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = async context =>
                    {
                        IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                            .Create(Configuration["AzureAdB2C:ClientId"])
                            .WithTenantId(Configuration["AzureAdB2C:TenantId"])
                            .WithClientSecret(Configuration["AzureAdB2C:ClientSecret"])
                            .Build();
                        // Create an authentication provider by passing in a client application and graph scopes.
                        ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

                        // Create a new instance of GraphServiceClient with the authentication provider.
                        GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                        var userId = context
                            .Principal
                            .Claims
                            .Where(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))
                            .First();

                        var request = await graphClient
                            .Users[userId.Value.ToString()]
                            .CheckMemberGroups(new string[] { Configuration["AzureAdB2C:AdminGroup"] })
                            .Request()
                            .PostAsync();

                        if (request.Count > 0)
                        {
                            var identity = context
                                .Principal
                                .Identities
                                .First();
                            identity.AddClaim(new Claim(identity.RoleClaimType, "Admin"));
                        }

                        return;
                    }
                };
            }).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", options =>
            {
                options.Events = new object
                {
                    
                };
            });

            services.AddAuthorization(c =>
            {
                c.AddPolicy("read", p =>
                {
                    p.RequireClaim("http://schemas.microsoft.com/identity/claims/scope", new List<string>
                    {
                        "read", "read write", "write read"
                    });
                });
                c.AddPolicy("write", p =>
                {
                    p.RequireClaim("http://schemas.microsoft.com/identity/claims/scope", new List<string>
                    {
                        "write", "read write", "write read"
                    });
                });
            });

            services.AddScoped<IUserService, UserService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Pedro's IoT API",
                    Version = "v1.0.0",
                    //Description = "Energy Management Solution for Industrial IoT"
                });
                //c.EnableAnnotations();
                //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "EMSfIIoT_API.xml"));

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Flows = new OpenApiOAuthFlows
                    {
                        /*Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://pedrorendeiro.b2clogin.com/pedrorendeiro.eu/b2c_1_signin/oauth2/v2.0/authorize", UriKind.Absolute),
                            Scopes = new Dictionary<string, string>
                            {
                                { Configuration["AzureAdB2C:AppIDURL"] + "/read", "Read permissions" },
                                { Configuration["AzureAdB2C:AppIDURL"] + "/write", "Write permissions" }
                            }
                        },*/
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://pedrorendeiro.b2clogin.com/pedrorendeiro.eu/b2c_1_signin/oauth2/v2.0/authorize", UriKind.Absolute),
                            TokenUrl = new Uri("https://pedrorendeiro.b2clogin.com/pedrorendeiro.eu/b2c_1_signin/oauth2/v2.0/token", UriKind.Absolute),
                            RefreshUrl = new Uri("https://pedrorendeiro.b2clogin.com/pedrorendeiro.eu/b2c_1_signin/oauth2/v2.0/token", UriKind.Absolute),
                            Scopes = new Dictionary<string, string>
                            {
                                { Configuration["AzureAdB2C:AppIDURL"] + "/read", "Read permissions" },
                                { Configuration["AzureAdB2C:AppIDURL"] + "/write", "Write permissions" }
                            }
                        }
                    }
                });
                c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Description = "Please enter your username and password",
                    Scheme = "basic",
                    In = ParameterLocation.Header
                });

                /*c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });*/

                c.OperationFilter<AuthenticationRequirementOperationsFilter>();
                c.OperationFilter<FixFromFormEncodingOperationFilter>();
                //c.OperationFilter<DefaultResponseOperationsFilter>();

            });

            services.AddDbContextPool<ApplicationDbContext>(
                options =>
                {
                    /*options.UseMySQL(Configuration.GetConnectionString("AWS"), providerOptions =>
                    {
                        providerOptions.MigrationsAssembly("IoTSuite.Server");
                    });*/
                    options.UseNpgsql(Configuration.GetConnectionString("PostgreSQL"), providerOptions =>
                    {
                        providerOptions.MigrationsAssembly("IoTSuite.Server");
                    });
                    if (WebHostEnvironment.IsDevelopment())
                    {
                        options.EnableSensitiveDataLogging();
                        options.UseLoggerFactory(LoggerFactory.Create(builder =>
                        {
                            builder.AddConsole();
                        }));
                    }
                }
            );

            services.AddSignalR();

            services.AddHttpContextAccessor();
            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });

            services
                .AddControllersWithViews(options =>
                {
                    options.EnableEndpointRouting = false;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = true;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = new Dictionary<string, string[]>();
                        foreach (var modelstate in context.ModelState)
                        {
                            if (modelstate.Value.Errors.Count > 0)
                            {
                                errors.Add(modelstate.Key, modelstate.Value.Errors.Select(error => error.ErrorMessage).ToArray());
                            }
                        }

                        var dto = new Response<Exception>(HttpStatusCode.BadRequest, errors);

                        return new ObjectResult(dto) { StatusCode = dto.StatusCode };
                    };
                });
            services.AddRazorPages();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            if (WebHostEnvironment.IsProduction())
            {
                services.AddSingleton(Configuration.GetSection("MQTT"));
                services.AddHostedService<MQTTTask>();
            }

            services.AddScoped<ITelegramWebhookService, TelegramWebhookService>();
            services.AddSingleton<ITelegramBotService, TelegramBotService>();
            services.Configure<TelegramBotConfiguration>(Configuration.GetSection("TelegramBotConfiguration"));

            _ = new GraphApiService(Configuration.GetSection("AzureAdB2C"));

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(
                        "https://localhost:5001",
                        "https://api.iot.pedrorendeiro.eu"
                        );
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext applicationDbContext, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                // Handles exceptions and generates a custom response body
                app.UseExceptionHandler("/api/errors/500");

                // Handles non-success status codes with empty body
                app.UseStatusCodePagesWithReExecute("/api/errors/{0}");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                var path = env.ContentRootPath;
                loggerFactory.AddFile($"{path}/Logs/Log.txt");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer>();
                    if (env.IsProduction())
                    {
                        swagger.Servers.Add(new OpenApiServer
                        {
                            Url = $"{httpReq.Scheme}://iot.pedrorendeiro.eu"
                        });
                    }
                    else if (env.IsDevelopment())
                    {
                        swagger.Servers.Add(new OpenApiServer
                        {
                            Url = $"{httpReq.Scheme}://{httpReq.Host.Value}"
                        });
                    }
                });

                c.RouteTemplate = "api/{documentName}/openApi.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api";
                c.SwaggerEndpoint("v1/openApi.json", "Pedro's IoT API v1");
                c.DocumentTitle = "API Documentation";
                c.EnableFilter();

                c.OAuthClientId(Configuration["AzureAdB2C:ClientId"]);
                c.OAuthClientSecret(Configuration["AzureAdB2C:ClientSecret"]);
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>
                {
                    {"prompt", "login"},
                    {"nonce", "defaultNonce"}
                });
                c.OAuthScopeSeparator(" ");
                c.OAuthUsePkce();

                c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                c.DefaultModelExpandDepth(1);
                c.DefaultModelsExpandDepth(-1);

                c.DisplayRequestDuration();
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                c.EnableDeepLinking();

                c.EnableValidator();

                c.ConfigObject.AdditionalItems.Add("persistAuthorization", true);

                //c.InjectStylesheet("/static/css/swaggerUI.css");
                //c.InjectJavascript("/custom.js", "text/javascript");
            });

            //applicationDbContext.Database.Migrate();

            ThingsController.UpdateThingsAndPolicies(applicationDbContext);

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvcWithDefaultRoute();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<SignalR>("/Hub/SignalR");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
