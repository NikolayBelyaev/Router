using Kit.Logging.Abstraction;
using Microsoft.AspNetCore.Components;
using Router.Core.Model.Configuration;
using Router.Executable.Admin.Enums;
using Router.Executable.Admin.Models;
using Router.Executable.Admin.Services;
using Router.Shared.Router;

namespace Router.Executable.Admin.Components;

public partial class RuleCard
{
    [Parameter] public RoutingServer RoutingServer { get; set; }
    [Parameter] public RoutingRule RoutingRule { get; set; }
    [Parameter] public List<string> TargetsNames { get; set; }
    [Parameter] public EventCallback OnChanged { get; set; }
    
    [Inject] private IRoutingService RoutingService { get; set; }
    [Inject] private IKitLogger Logger { get; set; }
    
    private bool _isVersionValid = true;

    private async Task OnVersionChange(RoutingRule routingRule, ChangeEventArgs args)
    {
        if (!ClientBuildVersion.TryParse(args.Value?.ToString() ?? string.Empty, out var version))
        {
            _isVersionValid = false;
            RoutingService.UpdateRuleVersion(routingRule, null);
            return;
        }

        _isVersionValid = true;

        var success = RoutingService.UpdateRuleVersion(routingRule, version);
        if (!success)
        {
            Logger.Error($"[Router.Admin] RuleCard. Rule version changing failed. " +
                         $"Server name: {RoutingServer.Name}, Version: {routingRule.Version}");
        }
    }

    private async Task OnUpdateTarget(ChangeEventArgs args)
    {
        RoutingService.UpdateRuleTarget(RoutingRule, args.Value?.ToString() ?? string.Empty); 
    }

    private async Task OnUpdateMode(ChangeEventArgs args)
    {
        if (!Enum.TryParse<UpdateMode>(args.Value?.ToString(), out var mode))
            return;
        
        RoutingService.UpdateRuleMode(RoutingRule, mode);
    }
    
    private async Task OnDeleteRule(RoutingServer routingServer, RoutingRule routingRule)
    {
        var success = RoutingService.DeleteRule(routingServer, routingRule.Id);
        if (!success)
        {
            Logger.Error($"[Router.Admin] RuleCard. Rule deleting failed. " +
                         $"Server name: {routingServer.Name}, Version: {routingRule.Version}");
        }
        
        await OnChanged.InvokeAsync();
    }
    
    private async Task OnTogglePlatform(RoutingRule routingRule, bool taken, ClientPlatform platform)
    {
        if (taken)
            return;
        
        var success = RoutingService.TogglePlatform(routingRule, platform);
        if (!success)
        {
            Logger.Error($"[Router.Admin] RuleCard. Rule toggle platform failed. " +
                         $"Server name: {RoutingServer.Name}, Version: {routingRule.Version}");
        }
    }
    
    private string GetFingerprint(RoutingRule rule)
    {
        if (rule == null)
            return null;
        
        return string.Join("|", rule.Id, rule.Version, string.Join(",", rule.Platforms.OrderBy(x => x)), 
            rule.Target, rule.UpdateMode);
    }

    private string GetRangeHintText(List<RoutingRule> allRules, RoutingRule routingRule)
    {
        if (routingRule.Version == null)
            return string.Empty;
        
        var nextVersion = allRules?
            .Where(r => r.Id != routingRule.Id && r.Version > routingRule.Version)
            .Select(r => (ClientBuildVersion?)r.Version)
            .OrderBy(v => v)
            .FirstOrDefault();
 
        return nextVersion == null
            ? $"≥ {routingRule.Version}"
            : $"≥ {routingRule.Version} and < {nextVersion}";
    }
}