using Kit.Logging.Abstraction;
using Microsoft.AspNetCore.Components;
using Router.Core.Model;
using Router.Executable.Admin.Models;
using Router.Executable.Admin.Models.Extensions;
using Router.Executable.Admin.Services;
using Router.Executable.Admin.Services.Validation;

namespace Router.Executable.Admin.Components.Pages;

public partial class Home
{
    [Inject] private ITargetsService TargetsService { get; set; }
    [Inject] private IRoutingService RoutingService { get; set; }
    [Inject] private IRoutingConfigurationValidator Validator { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IRoutingConfigurationService RoutingConfigurationService { get; set; }
    [Inject] private IKitLogger Logger { get; set; }
    
    private List<string> _targetsNames = [];
    private ValidationResult _validationResult = new();
    private bool _isLoading = true;
    private ConfirmationModal _configurationModal;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _isLoading = true;
            var targets = await RoutingConfigurationService.GetRouteTargetConfig();
            if (targets.Code != GetRouteTargetConfigurationResponse.ReturnCode.Ok)
                throw new Exception($"Targets loading failed: {targets.Code}");
            
            var routes = await RoutingConfigurationService.GetRoutingConfig();
            if (routes.Code != GetRoutingConfigurationResponse.ReturnCode.Ok)
                throw new Exception($"Routes loading failed: {routes.Code}");

            TargetsService.InitializeAsync(targets.Configuration);
            RoutingService.Initialize(routes.Configuration.ToServer());

            UpdateTargetsNames();
        }
        catch (Exception e)
        {
            Logger.Error($"[Router.Admin] Home. Loading data failed", e);
            _validationResult = ValidationResult.Failure(e.Message);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private Task OnTargetsChanged()
    {
        UpdateTargetsNames();
        StateHasChanged();
        
        return Task.CompletedTask;
    }

    private void UpdateTargetsNames()
    {
        _targetsNames = TargetsService.GetTargetsNames();
    }

    private async Task OnApply()
    {
        _validationResult = Validator.Validate();
        if (!_validationResult.IsValid)
            return;
        
        _configurationModal?.Show();
    }
    
    private async Task Apply()
    {
        _validationResult = Validator.Validate();
        if (!_validationResult.IsValid)
            return;

        try
        {
            _isLoading = true;

            // 1. Delete routing rule entries: removed platforms + all rules for removed servers
            foreach (var server in RoutingService.Servers)
            {
                if (server.ToBeDeleted)
                {
                    foreach (var rule in server.Rules)
                    {
                        foreach (var entryId in rule.ServerIds.Values)
                            await RoutingConfigurationService.DeleteRoutingRule(entryId);
                    }
                    
                    continue;
                }

                if (server.IsEditing) 
                    continue;

                foreach (var rule in server.Rules)
                {
                    var currentPlatforms = rule.Platforms.ToHashSet();
                    foreach (var (platform, entryId) in rule.ServerIds)
                    {
                        if (!currentPlatforms.Contains(platform) || rule.ToBeDeleted)
                            await RoutingConfigurationService.DeleteRoutingRule(entryId);
                    }
                }
            }

            // 2. Delete targets (not IsNew, since new ones were never on the server)
            foreach (var target in TargetsService.GetAllTargetsRaw().Where(t => t.ToBeDeleted && !t.IsNew))
                await RoutingConfigurationService.DeleteRouteTarget(target.OriginalTarget);

            // 3. Add new targets (before rules that reference them)
            foreach (var target in TargetsService.GetAllTargetsRaw().Where(t => t.IsNew && !t.ToBeDeleted))
                await RoutingConfigurationService.AddRouteTarget(target.Target, target.Address, target.Maintenance);

            // 4. Add new rule entries (platforms not present in ServerIds)
            foreach (var server in RoutingService.Servers.Where(s => !s.ToBeDeleted && !s.IsEditing))
            {
                foreach (var rule in server.Rules.Where(r => r.Version != null))
                {
                    foreach (var platform in rule.Platforms.Where(p => !rule.ServerIds.ContainsKey(p)))
                        await RoutingConfigurationService.AddRoutingRule(server.Name, platform, rule.Version!.Value, rule.Target, rule.UpdateMode);
                }
            }

            // 5. Update existing targets
            foreach (var target in TargetsService.GetAllTargetsRaw().Where(t => !t.IsNew && !t.ToBeDeleted && t.IsModified()))
                await RoutingConfigurationService.UpdateRouteTarget(target.OriginalTarget, target.Target, target.Address, target.Maintenance);

            // 6. Update existing rule entries (only if target or updateMode changed)
            foreach (var server in RoutingService.Servers.Where(s => !s.ToBeDeleted && !s.IsEditing))
            {
                foreach (var rule in server.Rules.Where(r => r.HasPropertyChanges() && r.Version != null))
                {
                    var currentPlatforms = rule.Platforms.ToHashSet();
                    foreach (var (platform, entryId) in rule.ServerIds)
                        if (currentPlatforms.Contains(platform))
                            await RoutingConfigurationService.UpdateRoutingRule(entryId, server.Name, platform, rule.Version!.Value, rule.Target, rule.UpdateMode);
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error($"[Router.Admin] Home. Apply data failed", e);
            _validationResult.Merge(ValidationResult.Failure(e.Message));
        }
        finally
        {
            ReloadPage();
        }
    }
    
    private void ReloadPage()
    {
        NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
    }
}