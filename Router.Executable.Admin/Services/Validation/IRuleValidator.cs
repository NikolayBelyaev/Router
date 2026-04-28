using Router.Executable.Admin.Models;

namespace Router.Executable.Admin.Services.Validation;

public interface IRuleValidator
{
    ValidationResult Validate(List<RoutingRule> rules, IReadOnlyCollection<string> targetNames);
}