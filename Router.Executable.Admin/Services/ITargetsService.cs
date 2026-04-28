using Router.Core.Model.Configuration;
using Router.Executable.Admin.Models;

namespace Router.Executable.Admin.Services;

public interface ITargetsService
{
    void InitializeAsync(RouteTargetConfiguration targetsConfig);
    List<TargetViewModel> GetTargets();
    TargetViewModel GetTarget(string id);
    List<string> GetTargetsNames();
    void AddTarget();
    void DeleteTarget(string id);
    List<TargetViewModel> GetAllTargets();
    List<TargetViewModel> GetAllTargetsRaw();
    bool HasChangedTargets();
}