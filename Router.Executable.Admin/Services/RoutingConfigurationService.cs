using System.Text;
using Kit.Logging.Abstraction;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Router.Core.Model;
using Router.Core.Model.Configuration;
using Router.Executable.Admin.Configuration;
using Router.Executable.Admin.Enums;
using Router.Shared.Router;

namespace Router.Executable.Admin.Services;

public class RoutingConfigurationService(HttpClient httpClient, IKitLogger logger, IOptions<RouterApiConfiguration> routerApiOptions)
    : IRoutingConfigurationService
{
    private readonly RouterApiConfiguration _routerApiConfig = routerApiOptions.Value;

    public async Task<GetRoutingConfigurationResponse> GetRoutingConfig()
    {
        var response = await httpClient.GetAsync(_routerApiConfig.GetRoutingConfigRoute);
        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<GetRoutingConfigurationResponse>(responseContent);
    }

    public async Task<GetRouteTargetConfigurationResponse> GetRouteTargetConfig()
    {
        return await ExecuteAsync(async () =>
            {
                var response = await httpClient.GetAsync(_routerApiConfig.GetRouteTargetConfigRoute);
                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<GetRouteTargetConfigurationResponse>(responseContent);
            }
        );
    }

    public async Task<SetRoutingConfigurationResponse> SaveRoutingConfiguration(RoutingConfiguration config)
    {
        return await ExecuteAsync(async () =>
            {
                var body = JsonConvert.SerializeObject(new SetRoutingConfigurationRequest(config));
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_routerApiConfig.SetRoutingConfigRoute, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                logger.Info($"[Router.Admin] SaveRoutingConfiguration done." +
                            $"Request body: {body}. " +
                            $"Response status code: {response.StatusCode}," +
                            $"Response content: {responseContent}");

                return JsonConvert.DeserializeObject<SetRoutingConfigurationResponse>(responseContent);
            }
        );
    }

    public async Task<SetRouteTargetConfigurationResponse> SaveRouteTargetConfig(RouteTargetConfiguration config)
    {
        return await ExecuteAsync(async () =>
            {
                var body = JsonConvert.SerializeObject(new SetRouteTargetConfigurationRequest(config));
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_routerApiConfig.SetRouteTargetConfigRoute, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                logger.Info($"[Router.Admin] SaveRouteTargetConfig done." +
                            $"Request body: {body}. " +
                            $"Response status code: {response.StatusCode}," +
                            $"Response content: {responseContent}");

                return JsonConvert.DeserializeObject<SetRouteTargetConfigurationResponse>(responseContent);
            }
        );
    }

    public async Task<SetMaintenanceOnResponse> SetMaintenanceOn(string pendingTargetName)
    {
        return await ExecuteAsync(async () =>
            {
                var body = JsonConvert.SerializeObject(new SetMaintenanceOnRequest(pendingTargetName));
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_routerApiConfig.SetMaintenanceOnRoute, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                logger.Info($"[Router.Admin] SetMaintenanceOn done." +
                            $"Request body: {body}. " +
                            $"Response status code: {response.StatusCode}," +
                            $"Response content: {responseContent}");

                return JsonConvert.DeserializeObject<SetMaintenanceOnResponse>(responseContent);
            }
        );
    }
    
    public async Task<SetMaintenanceOffResponse> SetMaintenanceOff(string pendingTargetName)
    {
        return await ExecuteAsync(async () =>
            {
                var body = JsonConvert.SerializeObject(new SetMaintenanceOffRequest(pendingTargetName));
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_routerApiConfig.SetMaintenanceOffRoute, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                logger.Info($"[Router.Admin] SetMaintenanceOff done." +
                            $"Request body: {body}. " +
                            $"Response status code: {response.StatusCode}," +
                            $"Response content: {responseContent}");

                return JsonConvert.DeserializeObject<SetMaintenanceOffResponse>(responseContent);
            }
        );
    }

    public async Task<AddRoutingRuleResponse> AddRoutingRule(string server, ClientPlatform platform, ClientBuildVersion version, string routeTarget, UpdateMode updateMode)
    {
        return await ExecuteAsync(async () =>
        {
            var body = JsonConvert.SerializeObject(new AddRoutingRuleRequest(
                server, platform.ToString().ToLower(), version.ToString(), routeTarget, updateMode));
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_routerApiConfig.AddRoutingRuleRoute, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            logger.Info($"[Router.Admin] AddRoutingRule done. Request: {body}. Response: {responseContent}");

            return JsonConvert.DeserializeObject<AddRoutingRuleResponse>(responseContent);
        });
    }

    public async Task<UpdateRoutingRuleResponse> UpdateRoutingRule(string id, string server, ClientPlatform platform, ClientBuildVersion version, string routeTarget, UpdateMode updateMode)
    {
        return await ExecuteAsync(async () =>
        {
            var body = JsonConvert.SerializeObject(new UpdateRoutingRuleRequest(
                id, server, platform.ToString().ToLower(), version.ToString(), routeTarget, updateMode));
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_routerApiConfig.UpdateRoutingRuleRoute, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            logger.Info($"[Router.Admin] UpdateRoutingRule done. Request: {body}. Response: {responseContent}");

            return JsonConvert.DeserializeObject<UpdateRoutingRuleResponse>(responseContent);
        });
    }

    public async Task<DeleteRoutingRuleResponse> DeleteRoutingRule(string id)
    {
        return await ExecuteAsync(async () =>
        {
            var body = JsonConvert.SerializeObject(new DeleteRoutingRuleRequest(id));
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_routerApiConfig.DeleteRoutingRuleRoute, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            logger.Info($"[Router.Admin] DeleteRoutingRule done. Id: {id}. Response: {responseContent}");

            return JsonConvert.DeserializeObject<DeleteRoutingRuleResponse>(responseContent);
        });
    }

    public async Task<AddRouteTargetResponse> AddRouteTarget(string target, string address, bool maintenance)
    {
        return await ExecuteAsync(async () =>
        {
            var body = JsonConvert.SerializeObject(new AddRouteTargetRequest(target, address, maintenance));
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_routerApiConfig.AddRouteTargetRoute, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            logger.Info($"[Router.Admin] AddRouteTarget done. Request: {body}. Response: {responseContent}");

            return JsonConvert.DeserializeObject<AddRouteTargetResponse>(responseContent);
        });
    }

    public async Task<UpdateRouteTargetResponse> UpdateRouteTarget(string originalTarget, string target, string address, bool maintenance)
    {
        return await ExecuteAsync(async () =>
        {
            var body = JsonConvert.SerializeObject(new UpdateRouteTargetRequest(originalTarget, target, address, maintenance));
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_routerApiConfig.UpdateRouteTargetRoute, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            logger.Info($"[Router.Admin] UpdateRouteTarget done. Request: {body}. Response: {responseContent}");

            return JsonConvert.DeserializeObject<UpdateRouteTargetResponse>(responseContent);
        });
    }

    public async Task<DeleteRouteTargetResponse> DeleteRouteTarget(string target)
    {
        return await ExecuteAsync(async () =>
        {
            var body = JsonConvert.SerializeObject(new DeleteRouteTargetRequest(target));
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_routerApiConfig.DeleteRouteTargetRoute, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            logger.Info($"[Router.Admin] DeleteRouteTarget done. Target: {target}. Response: {responseContent}");

            return JsonConvert.DeserializeObject<DeleteRouteTargetResponse>(responseContent);
        });
    }

    private async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception e)
        {
            logger.Error("[Router.Admin] Error", e);
            return default;
        }
    }
}