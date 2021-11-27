using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace JaegerTracing;

public static class HostBuilderExtensions
{
    public static WebApplicationBuilder AddJaegerTracing(this WebApplicationBuilder builder, params string[] sources)
    {
        var name = builder.Configuration.GetValue<string>("Jaeger:ServiceName");
        var host = builder.Configuration.GetValue<string>("Jaeger:Host");
        var port = builder.Configuration.GetValue<int>("Jaeger:Port");

        if (name is not { Length: > 0 } || host is not { Length: > 0 } || port == 0) return builder;

        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(name);

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resourceBuilder);
            options.AttachLogsToActivityEvent();
        });

        builder.Services.AddOpenTelemetryTracing(tracing =>
        {
            tracing.SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddGrpcClientInstrumentation();

            if (sources is { Length: > 0 })
            {
                tracing.AddSource(sources);
            }

            tracing.AddJaegerExporter(jaeger =>
            {
                jaeger.AgentHost = host;
                jaeger.AgentPort = port;
            });
        });

        return builder;
    }
}