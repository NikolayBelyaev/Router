using Router.Core.Model.Configuration;

namespace Router.Executable.Admin.Models.Extensions;

public static class TargetsConfigurationExtensions
{
    public static List<TargetViewModel> ToView(this RouteTargetConfiguration routeTargetConfiguration)
    {
        if (routeTargetConfiguration?.Entries == null)
            return [];

        return routeTargetConfiguration.Entries
            .Select(x => new TargetViewModel(target: x.Target, x.Address, x.Maintenance, originalTarget: x.Target))
            .ToList();
    }

    public static RouteTargetConfiguration ToRouteTargetConfiguration(this List<TargetViewModel> targets)
    {
        if (targets == null)
            return new RouteTargetConfiguration([]);

        return new RouteTargetConfiguration(
                targets
                    .Where(x => !x.ToBeDeleted)
                    .Select(x => new RouteTargetConfigurationEntry(x.Target, x.Address, x.Maintenance))
                    .ToArray());
    }
}