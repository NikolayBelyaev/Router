using System;

namespace Kit.Exe.Environment
{
    public class BuildVersionEnvironment
    {
        private const string EnvironmentVariableName = "BUILD_VERSION";
        
        public int BuildVersion { get; }

        public BuildVersionEnvironment()
        {
            var environment = System.Environment.GetEnvironmentVariable(EnvironmentVariableName);
            
            if (string.IsNullOrEmpty(environment))
            {
                throw new Exception($"Environment variable {EnvironmentVariableName} is not set");
            }

            BuildVersion = int.Parse(environment);
        }
    }
}