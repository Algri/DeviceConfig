using HealthChecks.UI.Client;
using IoT.RPiController.Data;
using IoT.RPiController.Data.Repositories.Abstractions;
using IoT.RPiController.Data.Repositories.Implementations;
using IoT.RPiController.Services.Hubs;
using IoT.RPiController.Services.Services.Abstractions;
using IoT.RPiController.Services.Services.Implementations;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using IoT.RPiController.Services.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IoT.RPiController.Services.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using IoT.RPiController.Services.Services.Implementations.Mocks;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

const string iotPolicies = "IotPolicies";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions<AuthConfigOptions>()
    .Bind(builder.Configuration.GetSection(AuthConfigOptions.AuthConfig));
builder.Services.AddOptions<BusConfigOptions>()
    .Bind(builder.Configuration.GetSection(BusConfigOptions.BusConfig));
builder.Services.AddOptions<LoggerConfigOptions>()
    .Bind(builder.Configuration.GetSection("LoggerPath"));

var logger = builder.Configuration
    .GetSection(LoggerConfigOptions.LoggerConfig)
    .Get<LoggerConfigOptions>();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.GrafanaLoki(
            logger.Path ?? "http://localhost:3100",
            new List<LokiLabel>()
            {
                new()
                {
                    Key = logger.LokiServiceKey ?? string.Empty,
                    Value = logger.LokiServiceValue ?? string.Empty
                }
            });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authConfig = builder.Configuration.GetSection(AuthConfigOptions.AuthConfig)
            .Get<AuthConfigOptions>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = authConfig.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = authConfig.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/ioTHub")))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "IotDevices", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

    option.DocInclusionPredicate((docName, description) => true);
    option.TagActionsBy(api => new List<string> { api.GroupName });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    option.IncludeXmlComments(xmlPath);
});

builder.Services.AddAutoMapper(
    typeof(RelayModuleMappingProfile),
    typeof(InputModuleMappingProfile));

builder.Services.AddDbContext<RPiContext>(options =>
{
    var folder = Environment.SpecialFolder.LocalApplicationData;
    var path = Environment.GetFolderPath(folder);
    var databasePath = Path.Join(path, "ModuleConfig.db");

    options.UseSqlite($"Data Source={databasePath}");
    Console.WriteLine("Database path: " + databasePath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(iotPolicies,
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.SetIsOriginAllowed((host) => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition");
        });
});

builder.Services.AddControllers().AddJsonOptions(
    options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);

builder.Services.AddSingleton<ITimerTokenService, TimerTokenService>();

#region Scoped Services

builder.Services.AddScoped<IModuleConfigurationRepository, ModuleConfigurationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITimerValueRepository, TimerValueRepository>();
builder.Services.AddScoped<IRelayInfoRepository, RelayInfoRepository>();
builder.Services.AddScoped<IGeneralConfigurationRepository, GeneralConfigurationRepository>();

#endregion

#region Transient Services

builder.Services.AddTransient<IHealthCheckService, HealthCheckService>();
builder.Services.AddTransient<IEventService, EventService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRelayInfoService, RelayInfoService>();
builder.Services.AddTransient<IJsonSerializationService, JsonSerializationService>();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    builder.Services.AddTransient<IRelayModuleService, RelayModuleService>();
    builder.Services.AddTransient<IInputModuleService, InputModuleService>();
    builder.Services.AddTransient<IRelayService, RelayService>();
    builder.Services.AddTransient<IPowerBusService, PowerBusService>();
    builder.Services.AddTransient<IOneWireService, OneWireService>();
}
else
{
    builder.Services.AddTransient<IRelayModuleService, RelayModuleServiceMock>();
    builder.Services.AddTransient<IInputModuleService, InputModuleServiceMock>();
    builder.Services.AddTransient<IRelayService, RelayServiceMock>();
    builder.Services.AddTransient<IPowerBusService, PowerBusServiceMock>();
    builder.Services.AddTransient<IOneWireService, OneWireServiceMock>();
}

#endregion

builder.Services.AddSignalR();
builder.Services.AddHealthChecks();

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseStaticFiles();

// Remote logging with Serilog
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

// Add global exception handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        // Customize your error response for a 500 status code
        var errorResponse = new
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "An error occurred while processing your request.",
            Status = context.Response.StatusCode,
            TraceId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier,
            Errors = new Dictionary<string, List<string>>
            {
                { "$", [exception?.Message] }
            }
        };

        var jsonResponse = JsonConvert.SerializeObject(errorResponse);
        await context.Response.WriteAsync(jsonResponse);
    });
});

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors(iotPolicies);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<IoTHub>("/ioTHub").RequireAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();