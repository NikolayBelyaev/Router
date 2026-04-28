namespace Router.Core.Model
{
    public class SetMaintenanceOffRequest
    {
        public string RouteTarget { get; }
        
        public SetMaintenanceOffRequest(string routeTarget)
        {
            RouteTarget = routeTarget;
        }
    }
}