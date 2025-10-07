using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Ical.Net;
using Microsoft.Extensions.DependencyInjection;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.Ical.Tests.Utils;

namespace TaskConverter.Plugin.Ical.Tests;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IConversionService<List<Calendar>>, ConversionService>();
        services.AddTransient<ISettingsProvider, TestSettingsProvider>();
        services.AddTransient<IFileSystem, MockFileSystem>();
    }
}
