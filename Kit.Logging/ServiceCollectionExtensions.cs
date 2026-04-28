using System.Reflection;
using Kit.Logging.Abstraction;
using Kit.Logging.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Kit.Logging
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsoleLogging(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient<IKitLogger, ConsoleLogger>();

            return serviceCollection;
        }

        public static IServiceCollection AddKitLogging(this IServiceCollection serviceCollection)
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            serviceCollection
                .TryAddTransient<IKitLogger>(provider =>
                    new KitLogger(
                        provider
                            .GetService<ILoggerFactory>()
                            .CreateLogger(entryAssembly.EntryPoint.DeclaringType)
                    )
                );

            return serviceCollection;
        }
    }
}