namespace Router.Core.Model
{
    public class DeleteRouteTargetRequest
    {
        public string Target { get; }

        public DeleteRouteTargetRequest(string target)
        {
            Target = target;
        }
    }
}
