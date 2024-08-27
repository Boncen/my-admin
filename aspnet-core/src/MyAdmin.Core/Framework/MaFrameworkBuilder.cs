using System.Reflection;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MyAdmin.ApiHost.Swagger;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Framework.Attribute;
using MyAdmin.Core.Framework.Filter;
using MyAdmin.Core.Logger;
using MyAdmin.Core.Options;
using MyAdmin.Core.Repository;
using MyAdmin.Core.Utilities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyAdmin.Core.Framework;

public static class MaFrameworkBuilder
{
    public static IServiceCollection AddMaFramework(this IServiceCollection service, ConfigurationManager configuration,
        Action<Core.MaFrameworkBuilder> builderAction = null, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0) 
            assemblies = new[] { Assembly.GetEntryAssembly() };
        var builder = new Core.MaFrameworkBuilder(service, assemblies);
        builderAction?.Invoke(builder);
        service.AddSingleton(builder);

        if (builder.UseBuildInDbContext)
        {
            HandleAddBuildInDbContext(service, configuration);
        }
        
        AddController(service);

        AddSwagger(service);
        
        AddRepository(service);

        AutoRegisterService(service, assemblies);
        
        #region log
        service.TryAddSingleton<ILogger, MyAdmin.Core.Logger.Logger>();
        #endregion

        AddOptions(service, configuration);
       
        AddDapper(service);
        
        return service;
    }

    private static void AddSwagger(IServiceCollection service)
    {
        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen();
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
        var useVersioningStr = configuration[$"{nameof(Core.Conf.Setting.MaFrameworkOptions)}:{nameof(Core.Conf.Setting.MaFrameworkOptions.UseApiVersioning)}"];
        if (string.IsNullOrEmpty(useVersioningStr) || !bool.TryParse(useVersioningStr, out bool useApiVersioning) || !useApiVersioning)
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
                    service.AddDbContext<MaDbContext>( dbContextOptions => dbContextOptions
                        .UseMySql(configuration["ConnectionStrings:Default"],  serverVersion)
                        .ConfigureWarnings((configurationBuilder => configurationBuilder.Throw()))
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors());
                    service.AddKeyedTransient<DbContext,MaDbContext>(nameof(MyAdmin.Core.Repository.MaDbContext));
                    break;
                case DBType.MsSql:
                    throw new UnSupposedFeatureException();
                    break;
                case DBType.Postgre:
                    throw new UnSupposedFeatureException();
                    break;
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
