using Microsoft.EntityFrameworkCore;
using MyAdmin.ApiHost;
using MyAdmin.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.SetupOptions(builder.Configuration);
builder.Services.UseApiVersioning(builder.Configuration);
builder.Services.UseLogger();
// db
var serverVersion = new MySqlServerVersion(new Version(9, 0, 0));
builder.Services.AddDbContext<MaDbContext>(
           dbContextOptions => dbContextOptions
               .UseMySql(builder.Configuration["ConnectionStrings:MySQL"], serverVersion)
               .LogTo(Console.WriteLine, LogLevel.Information)
               .EnableSensitiveDataLogging()
               .EnableDetailedErrors()
       );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.SetupSwaggerUI(builder.Configuration);
}

app.UseErrorHandleMiddleware();
app.UseHttpsRedirection();
app.MapControllers();



app.Run();