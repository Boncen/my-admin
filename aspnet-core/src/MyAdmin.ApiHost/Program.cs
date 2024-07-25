using MyAdmin.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.SetupOptions(builder.Configuration);
builder.Services.UseApiVersioning(builder.Configuration);
builder.Services.UseLogger();

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