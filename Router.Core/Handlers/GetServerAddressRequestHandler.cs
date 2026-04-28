using System;
using System.Linq;
using System.Threading.Tasks;
using Router.Core.ConfigurationStorage;
using Router.Core.Handlers.Abstraction;
using Router.Core.Model.Configuration;
using Router.Shared.Router;

namespace Router.Core.Handlers
{
    public class GetServerAddressRequestHandler 
        : RequestHandler<GetServerAddressRequest, GetServerAddressResponse, GetServerAddressResponse.ReturnCode>
    {
        protected override GetServerAddressResponse.ReturnCode Ok => GetServerAddressResponse.ReturnCode.Ok;
        protected override GetServerAddressResponse.ReturnCode BadRequest => GetServerAddressResponse.ReturnCode.IncorrectVersion;
        protected override GetServerAddressResponse.ReturnCode InternalServerError => GetServerAddressResponse.ReturnCode.InternalServerError;
        
        protected override Task<GetServerAddressResponse.ReturnCode> Validate(
            GetServerAddressRequest addressRequest,
            IRoutingConfigurationStorage configurationStorage)
        {
            if (!ClientBuildVersion.TryParse(addressRequest.ClientVersion, out _))
                return Task.FromResult(GetServerAddressResponse.ReturnCode.IncorrectVersion);

            if (string.IsNullOrEmpty(addressRequest.Server.Trim()))
                return Task.FromResult(GetServerAddressResponse.ReturnCode.IncorrectServer);

            if (string.IsNullOrEmpty(addressRequest.ClientPlatform))
                return Task.FromResult(GetServerAddressResponse.ReturnCode.IncorrectPlatform);
            
            return Task.FromResult(Ok);
        }

        protected override GetServerAddressResponse RespondWithError(GetServerAddressResponse.ReturnCode errorCode)
        {
            return new GetServerAddressResponse(errorCode, null);
        }

        protected override async Task<GetServerAddressResponse> ProcessValidRequest(
            GetServerAddressRequest addressRequest,
            IRoutingConfigurationStorage configurationStorage
        )
        {
            var routesArray = await configurationStorage.GetRouting(addressRequest.Server.ToLower());
            
            if (routesArray == null || routesArray.Length == 0)
                return new GetServerAddressResponse(GetServerAddressResponse.ReturnCode.IncorrectServer, null);

            routesArray = routesArray
                .GroupBy(x => x.Platform)
                .First(x => x.Key == addressRequest.ClientPlatform || addressRequest.ClientPlatform == ClientPlatformConstants.Editor)
                .ToArray();
            
            if (routesArray.Length == 0)
                return new GetServerAddressResponse(GetServerAddressResponse.ReturnCode.IncorrectPlatform, null);

            var version = ClientBuildVersion.Parse(addressRequest.ClientVersion);
            var suitable = routesArray
                .Select(x => new { Route = x, ClientVersion = ClientBuildVersion.Parse(x.ClientVersion)})
                .OrderBy(x => x.ClientVersion)
                .LastOrDefault(x => x.ClientVersion <= version);
            
            if (suitable == null)
                return new GetServerAddressResponse(GetServerAddressResponse.ReturnCode.UpdateRequired, null);

            var suitableRouteTarget = await configurationStorage.GetRouteTarget(suitable.Route.RouteTarget.ToLower());

            var successCode = ToReturnCode(suitable.Route.UpdateMode);
            var serverAddress = ShouldRouteToServer(successCode)
                ? suitableRouteTarget.Address
                : null;
            
            return suitableRouteTarget.Maintenance
                ? new GetServerAddressResponse(GetServerAddressResponse.ReturnCode.Maintenance, null)
                : new GetServerAddressResponse(successCode, serverAddress);
        }

        private bool ShouldRouteToServer(GetServerAddressResponse.ReturnCode code)
        {
            switch (code)
            {
                case GetServerAddressResponse.ReturnCode.Ok:
                case GetServerAddressResponse.ReturnCode.UpdateRecommended:
                    return true;
                default:
                    return false;
            }
        }

        private GetServerAddressResponse.ReturnCode ToReturnCode(UpdateMode mode)
        {
            switch (mode)
            {
                case UpdateMode.UpToDate:
                    return GetServerAddressResponse.ReturnCode.Ok;
                case UpdateMode.UpdateRecommended:
                    return GetServerAddressResponse.ReturnCode.UpdateRecommended;
                case UpdateMode.UpdateRequiredOfflineAllowed:
                    return GetServerAddressResponse.ReturnCode.UpdateRequiredOfflineAllowed;
                case UpdateMode.UpdateRequired:
                    return GetServerAddressResponse.ReturnCode.UpdateRequired;
                default:
                    throw new ArgumentException($"Unknown UpdateMode - ${mode}");
            }
        }
    }
}