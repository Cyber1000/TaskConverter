using Converter.Model.Mapper;
using Converter.Plugin.GTD.Mapper;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace Converter.Tests;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IClock, ConverterClock>();
        services.AddTransient<IConverter, Plugin.GTD.Mapper.Converter>();
        services.AddTransient<IConverterDateTimeZoneProvider, TestDateTimeZoneProvider>();
    }
}
