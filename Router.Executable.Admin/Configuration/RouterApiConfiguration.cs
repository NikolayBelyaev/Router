namespace Router.Executable.Admin.Configuration;

public class RouterApiConfiguration
{
    public string BaseUrl { get; set; }
    public string GetRoutingConfigRoute { get; set; }
    public string GetRouteTargetConfigRoute { get; set; }
    public string SetRoutingConfigRoute { get; set; }
    public string SetRouteTargetConfigRoute { get; set; }
    public string SetMaintenanceOnRoute { get; set; }
    public string SetMaintenanceOffRoute { get; set; }
    public string AddRoutingRuleRoute { get; set; }
    public string UpdateRoutingRuleRoute { get; set; }
    public string DeleteRoutingRuleRoute { get; set; }
    public string AddRouteTargetRoute { get; set; }
    public string UpdateRouteTargetRoute { get; set; }
    public string DeleteRouteTargetRoute { get; set; }
    public string AuthorizationToken { get; set; }
}