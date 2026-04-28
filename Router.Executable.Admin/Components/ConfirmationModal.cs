using Microsoft.AspNetCore.Components;

namespace Router.Executable.Admin.Components;

public partial class ConfirmationModal
{
    [Parameter] public string Title { get; set; } = "Confirm";
    [Parameter] public string Message { get; set; } = "Are you sure?";
    [Parameter] public EventCallback OnConfirmed { get; set; }

    private bool IsVisible { get; set; } = false;

    public void Show()
    {
        IsVisible = true;
        StateHasChanged();
    }

    public void Hide()
    {
        IsVisible = false;
        StateHasChanged();
    }

    private async Task Confirm()
    {
        IsVisible = false;
        await OnConfirmed.InvokeAsync(null);
    }
}