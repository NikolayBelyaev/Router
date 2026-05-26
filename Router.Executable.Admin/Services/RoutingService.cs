using Router.Core.Model.Configuration;
using Router.Executable.Admin.Enums;
using Router.Executable.Admin.Models;
using Router.Shared.Router;

namespace Router.Executable.Admin.Services;

public class RoutingService : IRoutingService
{
    public List<RoutingServer> Servers { get; private set; } = [];

    public void Initialize(List<RoutingServer> servers)
    {
        Servers = servers.ToList();
    }

    public void AddServer()
    {
        Servers.Add(new RoutingServer ($"new-server-{Servers.Count + 1}", null, true));
    }

    public void RemoveServer(string id)
    {
        var server = Servers.First(s => s.Id == id);
        server.ToBeDeleted = true;
    }

    public void RestoreServer(string id)
    {
        var server = Servers.First(s => s.Id == id);
        server.ToBeDeleted = false;
    }

    public void AddRule(string serverId)
    {
        var server = GetServer(serverId);
        server?.Rules.Add(new RoutingRule(server.Name));
    }
    
    public bool UpdateRuleVersion(RoutingRule routingRule, ClientBuildVersion? version)
    {
        if (routingRule == null)
            return false;

        if (!routingRule.IsNew)
            return false;

        routingRule.Version = version;
        return true;
    }
    
    public bool UpdateRuleTarget(RoutingRule routingRule, string target)
    {
        if (routingRule == null)
            return false;
        
        routingRule.Target = target;
        return true;
    }
    
    public bool UpdateRuleMode(RoutingRule routingRule, UpdateMode mode)
    {
        if (routingRule == null)
            return false;
        
        routingRule.UpdateMode = mode;
        return true;
    }
    
    public int GetRulesCount(string target)
    {
        return Servers?
                   .Where(x => !x.ToBeDeleted)
                   .SelectMany(r => r.Rules)
                   .Count(r => r.Target == target && r.Platforms != null && r.Platforms.Count != 0)
               ?? 0;
    }
    
    public bool DeleteRule(RoutingServer routingServer, string ruleId)
    {
        if (routingServer?.Rules == null || string.IsNullOrWhiteSpace(ruleId))
            return false;
        
        var rule = GetRule(routingServer.Id, ruleId);
        if (rule == null)
            return false;

        rule.ToBeDeleted = true;
        return true;
    }

    public bool TogglePlatform(RoutingRule routingRule, ClientPlatform platform)
    {
        if (routingRule == null) 
            return false;

        if (!routingRule.Platforms.Remove(platform))
            routingRule.Platforms.Add(platform);

        return true;
    }

    public HashSet<ClientPlatform> GetTakenPlatforms(RoutingServer routingServer, ClientBuildVersion version, string excludeRuleId)
    {
        if (routingServer?.Rules == null)
            return new HashSet<ClientPlatform>();

        return routingServer.Rules
            .Where(x => x.Version == version && (excludeRuleId == null || x.Id != excludeRuleId))
            .SelectMany(x => x.Platforms)
            .Distinct()
            .ToHashSet();
    }
    
    private RoutingServer GetServer(string id)  
        => Servers.FirstOrDefault(s => s.Id == id);

    private RoutingRule GetRule(string serverId, string ruleId) 
        => GetServer(serverId)?.Rules.FirstOrDefault(r => r.Id == ruleId);
}