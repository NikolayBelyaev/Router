using Microsoft.Extensions.Configuration;

namespace Kit.Exe.Environment
{
    public interface IRuntimeEnvironment
    {
        string Environment { get; }
    }

    public static class RuntimeEnvironmentExtensions
    {
        public static IConfigurationRoot BuildConfiguration(this IRuntimeEnvironment runtimeEnvironment)
        {
            return new ConfigurationBuilder()
                .AddJsonFile("./.config/app.json")
                .AddJsonFile($"./.config/app.{runtimeEnvironment.Environment}.json", true)
                .Build();
        }

        public static bool IsLocal(this IRuntimeEnvironment hostingEnvironment)
        {
            return hostingEnvironment.Environment == Constants.HostingEnvironment.Local;
        }

        public static bool IsDebug(this IRuntimeEnvironment hostingEnvironment)
        {
            return hostingEnvironment.Environment == Constants.HostingEnvironment.Debug;
        }

        public static bool IsDevelopment(this IRuntimeEnvironment hostingEnvironment)
        {
            return hostingEnvironment.Environment == Constants.HostingEnvironment.Development;
        }

        public static bool IsProduction(this IRuntimeEnvironment hostingEnvironment)
        {
            return hostingEnvironment.Environment == Constants.HostingEnvironment.Production;
        }
    }
}