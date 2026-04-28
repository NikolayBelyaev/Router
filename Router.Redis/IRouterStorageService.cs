using System.Collections.Generic;
using System.Threading.Tasks;

namespace Router.Redis
{
    public interface IRouterStorageService
    {
        Task Push<TValue>(TValue value, string key);
        Task<TValue> Grab<TValue>(string key) where TValue : class;

        Task HashSet<TValue>(string hashKey, string field, TValue value);
        Task HashDelete(string hashKey, string field);
        Task HashClear(string hashKey);
        Task<Dictionary<string, TValue>> HashGetAll<TValue>(string hashKey) where TValue : class;
    }
}
