using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyAdmin.Core.Entity;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Framework.Attribute;
using MyAdmin.Core.Identity;
using MyAdmin.Core.Logger;
using MyAdmin.Core.Options;
using MyAdmin.Core.Repository;
using MyAdmin.Core.Utilities;

namespace MyAdmin.Core.Framework;

public static class MaFrameworkBuilder
{
    public static IServiceCollection AddMaFramework(this IServiceCollection service, ConfigurationManager configuration,
        Action<Core.MaFrameworkBuilder>? builderAction = null, params Assembly[] assemblies)
    {
        var config = new MaFrameworkOptions();
        var frameworkOptions = configuration.GetSection("MaFrameworkOptions");
        frameworkOptions.Bind(config);
        if (assemblies == null)
        {
            assemblies = new Assembly[]{ Assembly.GetEntryAssembly()! };
        }
        var builder = new Core.MaFrameworkBuilder(service, assemblies);
        
        var frameworkAssemble = Assembly.GetAssembly(typeof(MaFrameworkBuilder));
        if (frameworkAssemble != null)
        {
            builder.Assemblies.Add(frameworkAssemble);
        }
        
        service.AddSingleton<Core.MaFrameworkBuilder>(builder);
        builderAction?.Invoke(builder);
        AddFrameworkService(service);
        // service.AddSingleton(builder);
        AddOptions(service, configuration);
        if (config.UseBuildInDbContext == true)
        {
            HandleAddBuildInDbContext(service, configuration);
        }
        if (config.UseRateLimit == true)
        {
            AddRateLimit(service, config.RateLimitOptions);
        }
        if (config.UseJwtBearer == true)
        {
            AddJwtBearer(service, configuration);
        }
        if (config.Cache != null)
        {
            AddCache(service, config);
        }
        AddController(service);
        AddSwagger(service);
        AddRepository(service);
        AutoRegisterService(service, assemblies);
        AddDapper(service);
        AddEasyApi(service);
        

        return service;
    }

    private static void AddEasyApi(this IServiceCollection service)
    {
        service.TryAddScoped<EasyApi>();
    }

    private static void AddCache(this IServiceCollection service, MaFrameworkOptions config)
    {
        switch (config.Cache?.CacheType)
        {
            case CacheTypeEnum.Redis:
                throw new UnSupposedFeatureException();
            default:
                service.AddMemoryCache();
                service.TryAddSingleton<ICacheManager, MemeroryCacheManager>();
                service.TryAddSingleton(typeof(ICacheManager<>), typeof(MemeroryCacheManager<>));
                break;
        }
    }

    private static void AddFrameworkService(IServiceCollection service)
    {
        service.AddHttpContextAccessor();
        service.AddSingleton<JwtHelper>();
        service.TryAddSingleton<ILogger, MyAdmin.Core.Logger.Logger>();
        service.AddScoped<ICurrentUser, CurrentUser>();
    }

    private static void AddJwtBearer(IServiceCollection service, ConfigurationManager configuration)
    {
        service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true, //是否验证Issuer
                ValidIssuer = configuration["Jwt:Issuer"], //发行人Issuer
                ValidateAudience = true, //是否验证Audience
                ValidAudience = configuration["Jwt:Audience"], //订阅人Audience
                ValidateIssuerSigningKey = true, //是否验证SecurityKey
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!)), //SecurityKey
                ValidateLifetime = true, //是否验证失效时间
                ClockSkew = TimeSpan.FromHours(
                    Convert.ToInt16(configuration["Jwt:ExpireHour"])), //过期时间容错值，解决服务器端时间不同步问题（秒）
                RequireExpirationTime = true,
            };
        });
        service.AddAuthorization(options =>
        {
            options.AddPolicy("admin", (p) => p.RequireClaim(ClaimTypes.Role, "admin", "administrator"));
        });
    }

    private static void AddRateLimit(IServiceCollection service, MaRateLimitOptions? configRateLimitOptions)
    {
        if (configRateLimitOptions == null)
        {
            configRateLimitOptions = new MaRateLimitOptions();
        }

        service.AddRateLimiter(_ => _.AddSlidingWindowLimiter(policyName: Conf.ConstStrings.RateLimitingPolicyName,
            options =>
            {
                options.PermitLimit = configRateLimitOptions.PermitLimit;
                options.Window = TimeSpan.FromSeconds(configRateLimitOptions.Window);
                options.SegmentsPerWindow = configRateLimitOptions.SegmentsPerWindow;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = configRateLimitOptions.QueueLimit;
            }));
    }

    private static void AddSwagger(IServiceCollection service)
    {
        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen(x =>
        {
            x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            // 添加安全要求
            x.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        });
        // configure swagger
        // service.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        // service.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());
    }

    private static void AddDapper(IServiceCollection service)
    {
        service.AddScoped(typeof(DBHelper));
    }

    private static void AddOptions(IServiceCollection service, ConfigurationManager configuration)
    {
        service.Configure<LoggerOption>(configuration.GetSection(nameof(Core.Conf.Setting.Logger)));
        service.Configure<MaFrameworkOptions>(configuration.GetSection(nameof(Core.Conf.Setting.MaFrameworkOptions)));
    }

    public static void UseApiVersioning(this Core.MaFrameworkBuilder builder, ConfigurationManager configuration)
    {
        var useVersioningStr =
            configuration[
                $"{nameof(Core.Conf.Setting.MaFrameworkOptions)}:{nameof(Core.Conf.Setting.MaFrameworkOptions.UseApiVersioning)}"];
        if (string.IsNullOrEmpty(useVersioningStr) || !bool.TryParse(useVersioningStr, out bool useApiVersioning) ||
            !useApiVersioning)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            return;
        }

        var apiVersionBuilder = builder.Services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true; // 指定默认的版本
            o.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("ver"),
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader(["ver", "X-Version", "api-version"]),
                new MediaTypeApiVersionReader("ver"),
                new MediaTypeApiVersionReader("api-version")
            );
        });
        apiVersionBuilder.AddMvc().AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    private static void HandleAddBuildInDbContext(IServiceCollection service, ConfigurationManager configuration)
    {
        var dbTypeStr = configuration["MaFrameworkOptions:DBType"];
        var dbVersion = configuration["MaFrameworkOptions:DBVersion"];
        if (!Check.HasValue(dbTypeStr))
        {
            return;
        }

        if (DBType.TryParse(dbTypeStr, out DBType dbType))
        {
            switch (dbType)
            {
                case DBType.MySql:
                    var serverVersion = new MySqlServerVersion(dbVersion);
                    service.AddDbContext<MaDbContext>(dbContextOptions => dbContextOptions
                        .UseMySql(configuration["ConnectionStrings:Default"], serverVersion));
                    service.AddKeyedTransient<DbContext, MaDbContext>(nameof(MyAdmin.Core.Repository.MaDbContext));
                    break;
                case DBType.MsSql:
                    throw new UnSupposedFeatureException();
                case DBType.Postgre:
                    throw new UnSupposedFeatureException();
            }
        }
    }

    private static void AddController(IServiceCollection service)
    {
        //service.AddControllers(options => options.Filters.Add(typeof(ValidationFilter)));
        service.AddControllers();
    }

    private static void AddRepository(IServiceCollection service)
    {
        // service.TryAddScoped<DbContext, MaDbContext>();
        service.TryAddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
        service.TryAddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));
        service.TryAddScoped(typeof(IRepository<,,>), typeof(RepositoryBase<,,>));
    }

    private static void AutoRegisterService(IServiceCollection service, Assembly[] assemblies)
    {
        foreach (var assem in assemblies)
        {
            var allTypes = assem.GetTypes();
            foreach (var t in allTypes)
            {
                if (t.IsInterface)
                {
                    continue;
                }

                var singletonAttr = t.GetCustomAttribute<SingletonAttribute>();
                if (singletonAttr != null)
                {
                    var key = singletonAttr.Key;
                    if (Check.HasValue(key))
                    {
                        service.TryAddKeyedSingleton<string>(t, key);
                        continue;
                    }
                    else
                    {
                        service.TryAddSingleton(t);
                        continue;
                    }
                }

                var scopedAttr = t.GetCustomAttribute<ScopedAttribute>();
                if (scopedAttr != null)
                {
                    var key = scopedAttr.Key;
                    if (Check.HasValue(key))
                    {
                        service.TryAddKeyedScoped(t, key);
                        continue;
                    }
                    else
                    {
                        service.TryAddScoped(t);
                        continue;
                    }
                }

                var transientAttr = t.GetCustomAttribute<TransientAttribute>();
                if (transientAttr != null)
                {
                    var key = transientAttr.Key;
                    if (Check.HasValue(key))
                    {
                        service.TryAddKeyedTransient(t, key);
                        continue;
                    }
                    else
                    {
                        service.TryAddTransient(t);
                        continue;
                    }
                }

                var interfs = t.GetInterfaces();
                foreach (var i in interfs)
                {
                    if (i == typeof(ITransient))
                    {
                        service.TryAddTransient(t);
                        continue;
                    }

                    if (i == typeof(IScoped))
                    {
                        service.TryAddScoped(t);
                        continue;
                    }

                    if (i == typeof(ISingleton))
                    {
                        service.TryAddSingleton(t);
                    }
                }
            }
        }
    }
}