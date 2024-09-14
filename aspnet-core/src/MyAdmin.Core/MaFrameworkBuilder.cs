using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MyAdmin.Core;

public sealed class MaFrameworkBuilder
{
    public IServiceCollection Services { get; }
    public List<Assembly> Assemblies { get; }

    // public List<Action<InterceptorContext>> InterceptorRegistrarList { get; } = new List<Action<InterceptorContext>>();
    public MaFrameworkBuilder(IServiceCollection services, Assembly[] assemblies)
    {
        Services = services;
        Assemblies = assemblies.ToList();
    }
}