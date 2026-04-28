using Microsoft.AspNetCore.Components;
using Router.Core.Model.Configuration;
using Router.Executable.Admin.Models;
using Router.Executable.Admin.Models.Extensions;
using Router.Executable.Admin.Services;

namespace Router.Executable.Admin.Components;

public partial class Maintenance
{
    [Parameter] public EventCallback OnChanged { get; set; }
    
    [Inject] private IRoutingConfigurationService RoutingConfigurationService { get; set; }
    [Inject] private ITargetsService TargetsSvs { get; set; }
    
    private List<TargetViewModel> _targets;
    private ConfirmationModal _confirmationModal;

    private string _pendingTargetName;
    private bool _pendingMaintenanceOn;

    protected override async Task OnInitializedAsync()
    {
        _ = LoadTargets();
    }

    private async Task UpdateState()
    {
        var targets = await LoadTargets();
        TargetsSvs.InitializeAsync(targets);
        
        await OnChanged.InvokeAsync();
    }

    private async Task<RouteTargetConfiguration> LoadTargets()
    {
        try
        {
            var targets = await RoutingConfigurationService.GetRouteTargetConfig();

            _targets = targets.Configuration
                .ToView()
                .Where(t => t != null && t.Target != TargetsService.ReservedTarget)
                .ToList();

            return targets.Configuration;
        }
        catch
        {
            _targets = null;
            return null;
        }
        finally
        {
            await OnChanged.InvokeAsync();
        }
    }

    private void RequestSetMaintenance(string targetName, bool maintenanceOn)
    {
        _pendingTargetName = targetName;
        _pendingMaintenanceOn = maintenanceOn;
        _confirmationModal.Show();
    }

    private async Task Apply()
    {
        if (_pendingTargetName == null) 
            return;

        if (_pendingMaintenanceOn)
            await RoutingConfigurationService.SetMaintenanceOn(_pendingTargetName);
        else
            await RoutingConfigurationService.SetMaintenanceOff(_pendingTargetName);

        _pendingTargetName = null;
        var targets = await LoadTargets();
        TargetsSvs.InitializeAsync(targets);
        
        await OnChanged.InvokeAsync();
    }

}