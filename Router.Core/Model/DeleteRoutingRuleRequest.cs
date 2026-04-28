namespace Router.Core.Model
{
    public class DeleteRoutingRuleRequest
    {
        public string Id { get; }

        public DeleteRoutingRuleRequest(string id)
        {
            Id = id;
        }
    }
}
