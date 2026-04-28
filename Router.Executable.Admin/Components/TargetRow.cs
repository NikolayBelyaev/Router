using Microsoft.AspNetCore.Components;
using Router.Executable.Admin.Models;
using Router.Executable.Admin.Services;

namespace Router.Executable.Admin.Components;

public partial class TargetRow
{
    [Parameter] public string TargetId { get; set; }
    [Parameter] public EventCallback OnChanged { get; set; }
    
    [Inject] private IRoutingService RoutingService { get; set; }
    [Inject] private ITargetsService TargetsService { get; set; }

    private TargetViewModel _target;
    private bool _isModified = false;

    protected override async Task OnInitializedAsync()
    {
        UpdateTarget();
        await OnChanged.InvokeAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        UpdateTarget();
        _isModified = _target != null && _target.IsModified();
    }

    private void UpdateTarget()
    {
        var target = TargetsService.GetTarget(TargetId);
        if (target is { ToBeDeleted: true })
            _target = null;
        
        _target = target;
    }
    
    private async Task UpdateModificationState()
    {
        _isModified = _target != null && _target.IsModified();
        await OnChanged.InvokeAsync();
    }

    private async Task DeleteTarget(TargetViewModel target)
    {
        TargetsService.DeleteTarget(target.Id);
        UpdateTarget();
        await OnChanged.InvokeAsync();
    }

    private async Task OnChangeToggle(ChangeEventArgs args)
    {
        _target.Maintenance = (bool)(args?.Value ?? false);

        await UpdateModificationState();
    }

    private async Task OnChangeTarget(ChangeEventArgs args)
    {
        _target.Target = args?.Value?.ToString() ?? string.Empty;
        
        await UpdateModificationState();
    }
    
    private async Task OnChangeAddress(ChangeEventArgs args)
    {
        _target.Address = args?.Value?.ToString() ?? string.Empty;
        
        await UpdateModificationState();
    }
}