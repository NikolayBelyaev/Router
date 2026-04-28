using Router.Executable.Admin.Models;

namespace Router.Executable.Admin.Services.Validation;

public class RoutingValidator : IRoutingValidator
{
    private readonly ITargetsService _targetsService;
    private readonly IRuleValidator _ruleValidator;

    public RoutingValidator(ITargetsService targetsService, IRuleValidator ruleValidator)
    {
        _targetsService = targetsService;
        _ruleValidator = ruleValidator;
    }

    public ValidationResult Validate(List<RoutingServer> servers)
    {
        if (servers is not { Count: > 0 })
            return ValidationResult.Failure("Server list is null or empty");

        var result = new ValidationResult();
        var targetNames = new HashSet<string>(_targetsService.GetTargetsNames(), StringComparer.OrdinalIgnoreCase);

        foreach (var server in servers)
            result.Merge(Validate(server, targetNames));

        return result;
    }

    private ValidationResult Validate(RoutingServer server, HashSet<string> targetNames)
    {
        var result = new ValidationResult();

        if (string.IsNullOrEmpty(server.Name))
            result.Errors.Add("Server name is empty");

        if (server.Rules is not { Count: > 0 })
        {
            result.Errors.Add($"Server '{server.Name}' has empty rules list");
            return result;
        }

        result.Merge(_ruleValidator.Validate(server.Rules, targetNames));
        return result;
    }
}