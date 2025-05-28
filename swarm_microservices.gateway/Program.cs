using Ocelot.Middleware;
using swarm_microservices.gateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHealthChecks("/health");

app.UseOcelot(oc =>
        oc.PreQueryStringBuilderMiddleware = async (context, next) =>
        {
            var contextAccessor =
                context.RequestServices.GetRequiredService<IHttpContextAccessor>();
            contextAccessor.HttpContext = context;
            await next.Invoke();
        }
    )
    .Wait();

app.UseAuthorization();

app.MapControllers();

app.Run();
