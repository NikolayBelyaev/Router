using Router.Core.Model.Configuration;
using Router.Executable.Admin.Enums;
using Router.Shared.Router;

namespace Router.Executable.Admin.Models.Extensions;

public static class RoutingConfigurationExtensions
{
    public static List<RoutingServer> ToServer(this RoutingConfiguration config)
    {
        if (config?.Entries == null || config.Entries.Length == 0) 
            return [];
        
        var servers = config.Entries
            .GroupBy(e => e.Server)
            .Select(serverGroup =>
            {
                var rules = serverGroup
                    .GroupBy(e => new { e.ClientVersion, e.RouteTarget, e.UpdateMode })
                    .Select(ruleGroup => new RoutingRule(
                        ClientBuildVersion.Parse(ruleGroup.Key.ClientVersion),
                        platforms: ruleGroup
                            .Select(e => (ClientPlatform) Enum.Parse(typeof(ClientPlatform), e.Platform, true))
                            .ToList(),
                        target: ruleGroup.Key.RouteTarget,
                        updateMode: ruleGroup.Key.UpdateMode,
                        serverIds: ruleGroup.ToDictionary(
                            e => (ClientPlatform) Enum.Parse(typeof(ClientPlatform), e.Platform, true),
                            e => e.Id)
                    ))
                    .ToList();

                return new RoutingServer(serverGroup.Key, rules);
            })
            .ToList();

        return servers;
    }
    
    public static RoutingConfiguration ToRoutingConfiguration(this List<RoutingServer> servers)
    {
        if (servers == null)
            return new RoutingConfiguration([]);

        var entries = new List<RoutingConfigurationEntry>();

        foreach (var server in servers.Where(x => x is { ToBeDeleted: false, IsEditing: false } && x.Rules.Any(r => r.Platforms.Count != 0)))
        {
            if (server.Rules == null) 
                continue;

            foreach (var rule in server.Rules.OrderBy(r => r.Version))
            {
                if (rule.Version == null || rule.Platforms == null)
                    continue;
                
                entries.AddRange(rule.Platforms
                    .Select(platform => new RoutingConfigurationEntry(
                        id: string.Empty,
                        server: server.Name,
                        platform: platform.ToString().ToLower(),
                        clientVersion: rule.Version.ToString(),
                        routeTarget: rule.Target,
                        updateMode: rule.UpdateMode)));
            }
        }

        return new RoutingConfiguration(entries.ToArray());
    }
}