using TaskConverter.Model.Mapper;
using TaskConverter.Plugin.GTD.Mapper;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace TaskConverter.Tests;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IClock, ConverterClock>();
        services.AddTransient<IConverter, Plugin.GTD.Mapper.Converter>();
        services.AddTransient<IConverterDateTimeZoneProvider, TestDateTimeZoneProvider>();
    }
}
