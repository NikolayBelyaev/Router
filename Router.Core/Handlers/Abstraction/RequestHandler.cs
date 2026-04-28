using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Router.Core.ConfigurationStorage;

namespace Router.Core.Handlers.Abstraction
{
    public abstract class RequestHandler<TRequest, TResponse, TCode>
        where TRequest : class
        where TCode : Enum
    {
        protected abstract TCode Ok { get; }
        protected abstract TCode BadRequest { get; }
        protected abstract TCode InternalServerError { get; }
        
        public async Task<TResponse> Handle(string body, IRoutingConfigurationStorage configurationStorage)
        {
            try
            {
                TRequest request = null;

                if (!string.IsNullOrEmpty(body))
                {
                    try
                    {
                        request = JsonConvert.DeserializeObject<TRequest>(body);
                    }
                    catch
                    {
                        return RespondWithError(BadRequest);
                    }
                }

                var code = await Validate(request, configurationStorage);

                return Equals(code, Ok)
                    ? await ProcessValidRequest(request, configurationStorage)
                    : RespondWithError(code);
            }
            catch
            {
                return RespondWithError(InternalServerError);
            }
        }

        protected abstract Task<TCode> Validate(TRequest request, IRoutingConfigurationStorage configurationStorage);
        protected abstract TResponse RespondWithError(TCode errorCode);
        protected abstract Task<TResponse> ProcessValidRequest(
            TRequest request,
            IRoutingConfigurationStorage configurationStorage
        );
    }
}