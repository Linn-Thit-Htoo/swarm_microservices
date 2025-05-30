var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder);

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHealthChecks("/health");

app.UseAuthorization();

app.MapControllers();

app.Run();
