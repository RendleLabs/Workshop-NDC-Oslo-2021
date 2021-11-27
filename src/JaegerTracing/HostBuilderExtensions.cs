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

    public static IWebHostBuilder AddJaegerTracing(this IWebHostBuilder hostBuilder, params string[] sources)
    {
        string name = null;
        string host = null;
        int port = 0;
        ResourceBuilder resourceBuilder = null;

        hostBuilder.ConfigureLogging((context, logging) =>
        {
            name = context.Configuration.GetValue<string>("Jaeger:ServiceName");
            host = context.Configuration.GetValue<string>("Jaeger:Host");
            port = context.Configuration.GetValue<int>("Jaeger:Port");

            if (name is { Length: > 0 } && host is { Length: > 0 } && port > 0)
            {
                resourceBuilder = ResourceBuilder.CreateDefault()
                    .AddService(name);
                logging.AddOpenTelemetry(options =>
                {
                    options.SetResourceBuilder(resourceBuilder);
                    options.AttachLogsToActivityEvent();
                });
            }
        });

        if (resourceBuilder is not null)
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.AddOpenTelemetryTracing(tracing =>
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
            });
        }

        return hostBuilder;
    }
}