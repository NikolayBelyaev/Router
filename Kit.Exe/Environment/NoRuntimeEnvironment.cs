namespace Kit.Exe.Environment
{
    public class NoRuntimeEnvironment : IRuntimeEnvironment
    {
        public string Environment { get; } = string.Empty;
    }
}