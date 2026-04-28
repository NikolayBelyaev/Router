using Microsoft.AspNetCore.Components;
using Router.Executable.Admin.Services;

namespace Router.Executable.Admin.Components;

public partial class Targets
{
    [Parameter] public EventCallback OnChanged { get; set; }
    
    [Inject] private ITargetsService TargetsService { get; set; }

    private string _alert;

    private async Task AddTarget()
    {
        TargetsService.AddTarget();
        
        await OnChanged.InvokeAsync();
    }
}