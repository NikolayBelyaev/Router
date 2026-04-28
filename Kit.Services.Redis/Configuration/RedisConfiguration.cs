namespace Kit.Services.Redis.Configuration
{
    public class RedisConfiguration
    {
        public string Host { get; set; }
        public string Password { get; set; }

        public bool IsEmpty =>
            string.IsNullOrEmpty(Host) &&
            string.IsNullOrEmpty(Password);
        
        public string ConnectionString => string.IsNullOrEmpty(Password)
            ? Host
            : $"{Host},password={Password}";
    }
}