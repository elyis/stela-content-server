using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using STELA_CONTENT.App.Service;
using STELA_CONTENT.Core.Entities.Models;
using STELA_CONTENT.Core.IService;
using STELA_CONTENT.Infrastructure.Data;
using STELA_CONTENT.Infrastructure.Service;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);
var app = builder.Build();
ConfigureMiddleware(app);

ApplyMigrations(app);
InitDatabase(app);

app.MapGet("/", () => "Content server is working!");

app.Run();

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation(GetEnvVar("CONTENT_DB_CONNECTION_STRING"));
}

string GetEnvVar(string name) => Environment.GetEnvironmentVariable(name) ?? throw new Exception($"{name} is not set");

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // DotEnv.Load();
    var contentDbConnectionString = GetEnvVar("CONTENT_DB_CONNECTION_STRING");

    var jwtSecret = GetEnvVar("JWT_AUTH_SECRET");
    var jwtIssuer = GetEnvVar("JWT_AUTH_ISSUER");
    var jwtAudience = GetEnvVar("JWT_AUTH_AUDIENCE");

    var rabbitMqHostname = GetEnvVar("RABBITMQ_HOSTNAME");
    var rabbitMqUserName = GetEnvVar("RABBITMQ_USERNAME");
    var rabbitMqPassword = GetEnvVar("RABBITMQ_PASSWORD");

    var rabbitMqAdditionalServiceImageQueue = GetEnvVar("RABBITMQ_ADDITIONAL_SERVICE_IMAGE_QUEUE_NAME");
    var rabbitMqMemorialImageQueue = GetEnvVar("RABBITMQ_MEMORIAL_IMAGE_QUEUE_NAME");
    var rabbitMqPortfolioMemorialImageQueue = GetEnvVar("RABBITMQ_PORTFOLIO_MEMORIAL_IMAGE_QUEUE_NAME");
    var rabbitMqMaterialImageQueue = GetEnvVar("RABBITMQ_MATERIAL_IMAGE_QUEUE_NAME");

    services.AddControllers(e =>
    {
        e.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
    });

    services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience
        });

    services.AddAuthorization();

    ConfigureSwagger(services);

    services.AddDbContext<ContentDbContext>(options =>
    {
        options.UseNpgsql(contentDbConnectionString);
    });


    services.AddSingleton<IJwtService, JwtService>();
    services.AddSingleton<RabbitMqBackgroundService>(sp =>
    {
        var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
        return new RabbitMqBackgroundService(
            scopeFactory,
            rabbitMqHostname,
            rabbitMqUserName,
            rabbitMqPassword,
            rabbitMqAdditionalServiceImageQueue,
            rabbitMqMemorialImageQueue,
            rabbitMqPortfolioMemorialImageQueue,
            rabbitMqMaterialImageQueue,
            sp.GetRequiredService<ILogger<RabbitMqBackgroundService>>()
        );
    });

    services.AddScoped<IPlotPriceCalculationService, PlotPriceCalculationService>();
    services.AddScoped<IMaterialService, MaterialService>();
    services.AddScoped<IMemorialService, MemorialService>();
    services.AddScoped<IAdditionalServicesService, AdditionalServicesService>();
    services.AddScoped<IPortfolioMemorialService, PortfolioMemorialService>();

    services.AddHostedService(sp => sp.GetRequiredService<RabbitMqBackgroundService>());
}

void ConfigureSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "stela_content_api",
            Description = "Api",
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Bearer auth scheme",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        options.OperationFilter<SecurityRequirementsOperationFilter>();

        options.EnableAnnotations();
    });
}

void ApplyMigrations(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ContentDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

void InitDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ContentDbContext>();

    var countAdditionalServices = context.AdditionalServices.Count();
    if (countAdditionalServices != 0)
        return;

    context.AdditionalServices.AddRangeAsync(new List<AdditionalService>
    {
        new() { Name = "Мемориальные комплексы", Price = 10000 },
        new() { Name = "Изготовление скульптур", Price = 10000 },
        new() { Name = "Эксклюзивные памятники", Price = 10000 }
    });

    var materials = new List<MemorialMaterial>()
    {
        new() { Name = "Шанси Блэк", ColorName = "Черный"},
        new() { Name = "Змеевик", ColorName = "Зеленый"},
        new() { Name = "Габбро-Диабаз", ColorName = "Черный", Hex = "484848"},
        new() { Name = "Диабаз", ColorName = "Серый"},
        new() { Name = "Коелга", ColorName = "Белый"},
        new() { Name = "Абсолют Блэк", ColorName = "Черный"},
        new() { Name = "Олив Грин", ColorName = "Зеленый", Hex = "549079" },
        new() { Name = "Паданг Дарк", ColorName = "Серый" },
        new() { Name = "Роял Уайт", ColorName = "Белый"},
        new() { Name = "Чайна Марбал Уайт", ColorName = "Белый"},
        new() { Name = "Империал Рэд", ColorName = "Красный"},
        new() { Name = "Куру Грей", ColorName = "Серый"}
    };
    context.Materials.AddRange(materials);

    context.Memorials.AddRange(new List<Memorial>()
    {
        new()
        {
            Name = "Проект 1",
            Price = 40000,
            Description = "",
            StelaHeight = 1,
            StelaLength = 1,
            StelaWidth = 1,
            Materials = new List<MemorialMaterials>()
            {
                new() { Material = materials[0]}
            }
        },

        new()
        {
            Name = "Проект 2",
            Price = 40000,
            Description = "",
            StelaHeight = 1,
            StelaLength = 1,
            StelaWidth = 1,
            Materials = new List<MemorialMaterials>()
            {
                new() { Material = materials[3]},
            }
        },

        new()
        {
            Name = "Проект 3",
            Price = 40000,
            Description = "",
            StelaHeight = 1,
            StelaLength = 1,
            StelaWidth = 1,
            Materials = new List<MemorialMaterials>()
            {
                new() { Material = materials[5]},
                new() { Material = materials[9]},
            }
        },

        new()
        {
            Name = "Проект 4",
            Price = 40000,
            Description = "",
            StelaHeight = 1,
            StelaLength = 1,
            StelaWidth = 1,
            Materials = new List<MemorialMaterials>()
            {
                new() { Material = materials[10]},
                new() { Material = materials[11]},
            }
        }
    });

    var portfolioMemorials = new List<PortfolioMemorial>()
    {
        new() { CemeteryName = "Православное кладбище", Name = "МОНУМЕНТАЛЬНЫЙ МЕМОРИАЛЬНЫЙ КОМПЛЕКС", Description = "", Materials = new List<PortfolioMemorialMaterials>()
        {
            new() { Material = materials[0]}
        }},

        new() { CemeteryName = "Православное кладбище",Description = "", Name = "ЭКСКЛЮЗИВНЫЙ ПАМЯТНИК ИЗ ЗМЕЕВИКА", Materials = new List<PortfolioMemorialMaterials>()
        {
            new() { Material = materials[1]},
            new() { Material = materials[2]}
        }},

        new() { CemeteryName = "Православное кладбище",Description = "", Name = "МЕМОРИАЛЬНЫЙ КОМПЛЕКС", Materials = new List<PortfolioMemorialMaterials>()
        {
            new() { Material = materials[1]},
            new() { Material = materials[3]},
            new() { Material = materials[4]}
        }},

        new() { CemeteryName = "Православное кладбище",Description = "", Name = "МЕМОРИАЛЬНЫЙ КОМПЛЕКС", Materials = new List<PortfolioMemorialMaterials>()
        {
            new() { Material = materials[5]},
            new() { Material = materials[6]},
        }},

        new() { CemeteryName = "Православное кладбище",Description = "", Name = "АВТОРСКАЯ НАДГРОБНАЯ КОМПОЗИЦИЯ", Materials = new List<PortfolioMemorialMaterials>()
        {
            new() { Material = materials[7]},
        }},

        new() { CemeteryName = "Православное кладбище",Description = "", Name = "ЭЛИТНЫЙ МЕМОРИАЛЬНЫЙ КОМПЛЕКС", Materials = new List<PortfolioMemorialMaterials>()
        {
            new() { Material = materials[0]},
            new() { Material = materials[8]},
        }}
    };

    context.PortfolioMemorials.AddRange(portfolioMemorials);
    context.SaveChanges();
}