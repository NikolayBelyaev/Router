using Router.Core.Model.Configuration;
using Router.Executable.Admin.Models;
using Router.Executable.Admin.Models.Extensions;

namespace Router.Executable.Admin.Services;

public class TargetsService : ITargetsService
{
    public const string ReservedTarget = "maintenance";
    
    private List<TargetViewModel> _targets = [];

    public void InitializeAsync(RouteTargetConfiguration targetsConfig)
    {
        _targets = targetsConfig.ToView();
    }

    public List<TargetViewModel> GetAllTargets()
    {
        return _targets.Where(x => x is { ToBeDeleted: false }).ToList();
    }

    public List<TargetViewModel> GetTargets()
    {
        return _targets
            .Where(x => !x.ToBeDeleted && !x.Target.Equals(ReservedTarget, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
    }

    public TargetViewModel GetTarget(string id)
    {
        return _targets.FirstOrDefault(x => x.Id == id);
    }

    public List<string> GetTargetsNames()
    {
        return _targets
            .Where(x => !x.ToBeDeleted)
            .Select(x => x.Target)
            .Where(x => !x.Equals(ReservedTarget, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
    }

    public void AddTarget()
    {
        _targets.Add(new TargetViewModel(string.Empty, string.Empty, false, true));
    }

    public void DeleteTarget(string id)
    {
        foreach (var target in _targets.Where(x => x.Id == id))
        {
            target.ToBeDeleted = true;
        }
    }

    public List<TargetViewModel> GetAllTargetsRaw()
    {
        return _targets.ToList();
    }

    public bool HasChangedTargets()
    {
        return _targets.Any(x => x.IsNew || x.IsModified() || x.ToBeDeleted);
    }
}