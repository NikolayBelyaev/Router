using Kit.Exe;
using Kit.Logging.Abstraction;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Router.Executable.Admin.Models;
using Router.Executable.Admin.Services;

namespace Router.Executable.Admin.Components;

public partial class Routing
{
    [Parameter] public List<string> TargetsNames { get; set; }
    [Parameter] public Action OnAddServer { get; set; }
    [Parameter] public Action<string> OnRemoveServer { get; set; }
    [Parameter] public EventCallback OnChanged { get; set; }
    
    [Inject] private IRoutingService RoutingService { get; set; }
    [Inject] private IKitLogger Logger { get; set; }
    
    private RoutingServer _active;
    private bool _loaded = false;
    private ElementReference _inputReference;

    protected override async Task OnInitializedAsync()
    {
        _active = RoutingService.Servers
            .OrderByDescending(x => x.Name == Constants.HostingEnvironment.Production)
            .FirstOrDefault();
        
        _loaded = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await _inputReference.FocusAsync();
        }
        catch
        {
            //ignored
        }
    }

    private void Select(RoutingServer routingServer)
    {
        _active = routingServer;
    }
    
    private async Task HandleKeyDown(KeyboardEventArgs e, RoutingServer routingServer)
    {
        if (string.IsNullOrWhiteSpace(routingServer.Name))
            return;
        
        if (e.Key == "Enter")
        {
            await FinishEditing(routingServer);
        }
        else if (e.Key == "Escape")
        {
            routingServer.IsEditing = false;
        }
    }

    private async Task FinishEditing(RoutingServer routingServer)
    {
        if (!routingServer.IsEditing)
            return;

        routingServer.IsEditing = false;
        
        Select(routingServer);
    }

    private async Task OnAddRule(RoutingServer routingServer)
    {
        RoutingService.AddRule(routingServer.Id); 
        
        await OnChanged.InvokeAsync();
    }
    
    private async Task OnDeleteServer(RoutingServer routingServer)
    {
        RoutingService.RemoveServer(routingServer.Id); 
        
        await OnChanged.InvokeAsync();
    }

    private async Task OnRestoreServer(RoutingServer routingServer)
    {
        RoutingService.RestoreServer(routingServer.Id);
        
        await OnChanged.InvokeAsync();
    }
}