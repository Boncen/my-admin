using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using MyAdmin.ApiHost;
using MyAdmin.ApiHost.Db;
using MyAdmin.Core.Extensions;

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
               .LogTo(Console.WriteLine, LogLevel.Information)
               .EnableSensitiveDataLogging()
               .EnableDetailedErrors()
       );
builder.Services.AddTransient<Microsoft.EntityFrameworkCore.DbContext, MaDbContext>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.SetupSwaggerUI(builder.Configuration);
}

app.UseMaFramework(); // todo
app.UseErrorHandleMiddleware();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();