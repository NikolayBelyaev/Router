namespace Kit.Exe.Environment
{
    public class ConstantRuntimeEnvironment : IRuntimeEnvironment
    {
        public string Environment { get; }

        public ConstantRuntimeEnvironment(string environment)
        {
            Environment = environment;
        }
    }
}