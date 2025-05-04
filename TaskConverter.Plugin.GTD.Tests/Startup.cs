using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IClock, ConverterClock>();
        services.AddTransient<IConverter, Converter>();
        services.AddTransient<IConverterDateTimeZoneProvider, TestDateTimeZoneProvider>();
    }
}
