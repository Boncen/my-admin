using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyAdmin.ApiHost.Db;
using MyAdmin.Core.Extensions;
using MyAdmin.Core.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMaFramework( builder.Configuration, (o)=>{
    o.UseApiVersioning(builder.Configuration);
}, Assembly.GetExecutingAssembly());

// db
var serverVersion = new MySqlServerVersion(new Version(9, 0, 0));
builder.Services.AddDbContext<MaDbContext>(
           dbContextOptions => dbContextOptions
               .UseMySql(builder.Configuration["ConnectionStrings:MySQL"], serverVersion)
               .ConfigureWarnings((configurationBuilder => configurationBuilder.Throw()))
               // .LogTo(Console.WriteLine, LogLevel.Information)
               .EnableSensitiveDataLogging()
               .EnableDetailedErrors()
       );
builder.Services.AddTransient<Microsoft.EntityFrameworkCore.DbContext, MaDbContext>();
var app = builder.Build();

app.UseMaFramework(builder.Configuration);
app.UseHttpsRedirection();
app.MapControllers();

app.Run();