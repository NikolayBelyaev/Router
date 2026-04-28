using Router.Core.Model.Configuration;
using Router.Executable.Admin.Enums;
using Router.Executable.Admin.Models;
using Router.Shared.Router;

namespace Router.Executable.Admin.Services;

public interface IRoutingService
{
    void Initialize(List<RoutingServer> servers);
    List<RoutingServer> Servers { get; }
    void AddServer();
    void RemoveServer(string id);
    void RestoreServer(string id);
    void AddRule(string serverId);
    bool UpdateRuleVersion(RoutingRule routingRule, ClientBuildVersion? version);
    bool UpdateRuleTarget(RoutingRule routingRule, string target);
    bool UpdateRuleMode(RoutingRule routingRule, UpdateMode mode);
    int GetRulesCount(string target);
    bool DeleteRule(RoutingServer routingServer, string ruleId);
    bool TogglePlatform(RoutingRule routingRule, ClientPlatform platform);
    HashSet<ClientPlatform> GetTakenPlatforms(RoutingServer routingServer, ClientBuildVersion version, string excludeRuleId);
}