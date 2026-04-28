namespace Router.Shared.Router
{
    public class GetServerAddressResponse
    {
        public enum ReturnCode
        {
            Ok,
            UpdateRequired,
            UpdateRequiredOfflineAllowed,
            UpdateRecommended,
            IncorrectVersion,
            IncorrectServer,
            IncorrectPlatform,
            Maintenance,
            InternalServerError
        }
        
        public ReturnCode Code { get; }
        
        public string ServerAddress { get; }
        
        public GetServerAddressResponse(ReturnCode code, string serverAddress)
        {
            Code = code;
            ServerAddress = serverAddress;
        }
    }
}