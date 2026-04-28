using StackExchange.Redis;

namespace Router.Redis
{
    public interface IRouterRedisStorage
    {
        IDatabase RouterConfiguration { get; }
        void Dispose();
    }
}