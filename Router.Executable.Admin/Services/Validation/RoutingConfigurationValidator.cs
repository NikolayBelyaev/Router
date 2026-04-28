using Router.Executable.Admin.Models;

namespace Router.Executable.Admin.Services.Validation;

public class RoutingConfigurationValidator : IRoutingConfigurationValidator
{
    private readonly ITargetsValidator _targetsValidator;
    private readonly IRoutingValidator _routingValidator;
    private readonly ITargetsService _targetsService;
    private readonly IRoutingService _routingService;

    public RoutingConfigurationValidator(ITargetsValidator targetsValidator, IRoutingValidator routingValidator, 
        ITargetsService targetsService, IRoutingService routingService)
    {
        _targetsValidator = targetsValidator;
        _routingValidator = routingValidator;
        _targetsService = targetsService;
        _routingService = routingService;
    }

    public ValidationResult Validate()
    {
        var result = new ValidationResult();
        
        var targets = _targetsService.GetTargets();
        result.Merge(_targetsValidator.Validate(targets));

        var servers = _routingService.Servers.Where(x => !x.ToBeDeleted).ToList();
        result.Merge(_routingValidator.Validate(servers));

        return result;
    }
}