using Scalar.AspNetCore;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Telemetria com OpenTelemetry
//builder.Services.AddOpenTelemetry();

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Adiciona suporte ao Reverse Proxy
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));



// Telemetria com OpenTelemetry
//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracing =>
//    {
//        tracing
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation()
//            .AddOtlpExporter(); // Exporta para Jaeger, Zipkin ou outros
//    })
//    .WithMetrics(metrics =>
//    {
//        metrics
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation()
//            .AddRuntimeInstrumentation()
//            .AddProcessInstrumentation();
//    });


//builder.Services.AddReverseProxy()
//       .LoadFromMemory(new[]
//       {
//           new RouteConfig()
//           {
//               RouteId = "auth-route",
//               ClusterId = "auth-cluster",
//               Match = new RouteMatch()
//               {
//                   Path = "/api/v1/Authenticate/login"
//               }
//           }
//       },
//       new[]
//       {
//           new ClusterConfig()
//           {
//               ClusterId = "auth-cluster",
//               Destinations = new Dictionary<string, DestinationConfig>()
//               {
//                   { "auth-destination", new DestinationConfig() { Address = "http://localhost:44367" } }
//               }
//           }
//       });





var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

app.MapOpenApi();
app.MapReverseProxy();

app.MapScalarApiReference(o => o.WithTitle("Financial YARP").WithTheme(ScalarTheme.BluePlanet));


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Use(async (context, next) =>
{
    context.Request.EnableBuffering(); // Importante para ler o corpo da requisição
    var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
    context.Request.Body.Position = 0;

    app.Logger.LogInformation($"Request to YARP: {context.Request.Method} {context.Request.Path} Body: {body}");

    await next();
});


app.Run();
