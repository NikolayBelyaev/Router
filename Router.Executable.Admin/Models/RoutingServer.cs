namespace Router.Executable.Admin.Models;

public class RoutingServer(string name, List<RoutingRule> rules = null, bool isEditing = false)
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = name;
    public List<RoutingRule> Rules { get; set; } = rules ?? [];
    public bool IsEditing { get; set; } = isEditing;
    public bool ToBeDeleted { get; set; } = false;
}