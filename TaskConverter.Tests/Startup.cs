using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using TaskConverter.Model.Mapper;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Tests.Utils;

namespace TaskConverter.Tests;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IClock, ConverterClock>();
        services.AddTransient<IConverter, Converter>();
        services.AddTransient<IConverterDateTimeZoneProvider, TestDateTimeZoneProvider>();
    }
}
