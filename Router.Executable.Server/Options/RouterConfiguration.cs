namespace Router.Executable.Server.Options
{
    public class RouterConfiguration
    {
        public string Token { get; set; }
        public string AWSAccessKeyId  { get; set; }
        public string AWSSecret  { get; set; }
        public string DynamoDbTablePrefix { get; set; }
    }
}