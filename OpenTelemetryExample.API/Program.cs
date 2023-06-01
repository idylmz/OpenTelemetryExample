using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Exporter;
using OpenTelemetry.Contrib.Instrumentation.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
                    .ConfigureResource(otelBuilder => otelBuilder
                        .AddService(serviceName: "OpenTelemetryExample"))
                    .WithTracing(otelBuilder => otelBuilder
                        .AddAspNetCoreInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddConsoleExporter()
                        .AddZipkinExporter()
                        .ConfigureServices(services =>
                        {
                            services.Configure<ZipkinExporterOptions>(builder.Configuration.GetSection("OpenTelemetrySettings:ZipkinSettings"));
                            services.Configure<EntityFrameworkInstrumentationOptions>(builder.Configuration.GetSection("OpenTelemetrySettings:SQLTracingSettings"));
                        }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
