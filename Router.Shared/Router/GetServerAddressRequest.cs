using Newtonsoft.Json;

namespace Router.Shared.Router
{
    public class GetServerAddressRequest
    {
        [JsonConverter(typeof(LowercaseStringConverter))]
        public string Server { get; }

        [JsonConverter(typeof(LowercaseStringConverter))]
        public string ClientPlatform { get; }

        [JsonConverter(typeof(LowercaseStringConverter))]
        public string ClientVersion { get; }
        
        public GetServerAddressRequest(string server, string clientPlatform, string clientVersion)
        {
            Server = server;
            ClientPlatform = clientPlatform;
            ClientVersion = clientVersion;
        }
    }
}