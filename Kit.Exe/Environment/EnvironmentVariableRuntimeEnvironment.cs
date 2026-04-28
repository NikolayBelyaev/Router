using System;

namespace Kit.Exe.Environment
{
    public class EnvironmentVariableRuntimeEnvironment : IRuntimeEnvironment
    {
        private const string EnvironmentVariableName = "RUNTIME_ENVIRONMENT";
        
        public string Environment { get; }

        public EnvironmentVariableRuntimeEnvironment()
        {
            Environment = System.Environment.GetEnvironmentVariable(EnvironmentVariableName);
            
            if (null == Environment)
            {
                throw new Exception($"Environment variable {EnvironmentVariableName} is not set");
            }
        }
    }
}