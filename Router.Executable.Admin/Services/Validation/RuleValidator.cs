using Router.Executable.Admin.Models;

namespace Router.Executable.Admin.Services.Validation;

public class RuleValidator : IRuleValidator
{
    public ValidationResult Validate(List<RoutingRule> rules, IReadOnlyCollection<string> targetNames)
    {
        var result = new ValidationResult();
        
        if (rules is not { Count: > 0 })
        {
            result.Errors.Add("Rules list is empty");
            return result;
        }

        var targetSet = new HashSet<string>(targetNames, StringComparer.OrdinalIgnoreCase);

        foreach (var rule in rules)
        {
            var ruleResult = Validate(rule, targetSet);
            result.Merge(ruleResult);
        }

        return result;
    }

    private static ValidationResult Validate(RoutingRule rule, HashSet<string> targetNames)
    {
        if (rule is null)
            return ValidationResult.Failure("Rule is null");

        var result = new ValidationResult();
        
        if (rule.Version == null)
            result.Errors.Add("Rule has invalid version");

        if (rule.Platforms is not { Count: > 0 })
            result.Errors.Add($"Rule with version '{rule.Version}' has no any selected platforms");

        if (!targetNames.Contains(rule.Target))
            result.Errors.Add($"Rule with version '{rule.Version}' has invalid target: '{rule.Target}'");

        return result;
    }
}