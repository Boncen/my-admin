using System.Reflection;
using MyAdmin.Core.Framework;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMaFramework( builder.Configuration, (o)=>{
    // o.UseApiVersioning(builder.Configuration);
}, Assembly.GetExecutingAssembly());
var app = builder.Build();

app.UseMaFramework(builder.Configuration);
app.Run();