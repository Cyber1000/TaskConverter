using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using TaskConverter.Commons;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IClock, ConverterClock>();
        services.AddTransient<IConversionService<GTDDataModel>, ConversionService>();
        services.AddTransient<ISettingsProvider, TestSettingsProvider>();
        services.AddTransient<IFileSystem, MockFileSystem>();
        services.AddTransient<IKeyWordMapperService, KeyWordMapperService>();
    }
}
