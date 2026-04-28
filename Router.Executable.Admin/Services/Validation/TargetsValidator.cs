using Router.Executable.Admin.Models;

namespace Router.Executable.Admin.Services.Validation;

public class TargetsValidator : ITargetsValidator
{
    public ValidationResult Validate(List<TargetViewModel> targets)
    {
        if (targets is not { Count: > 0 })
            return ValidationResult.Failure("Targets cannot be empty");

        var result = new ValidationResult();

        foreach (var target in targets)
            result.Merge(Validate(target));

        var duplicatedTargets = targets
            .GroupBy(x => x.Target)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicatedTargets.Count != 0)
            result.Errors.Add($"Duplicated targets found: {string.Join(", ", duplicatedTargets)}");

        var duplicatedAddresses = targets
            .GroupBy(x => x.Address)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicatedAddresses.Count != 0)
            result.Errors.Add($"Duplicated addresses found: {string.Join(", ", duplicatedAddresses)}");

        return result;
    }

    private static ValidationResult Validate(TargetViewModel target)
    {
        if (target is null)
            return ValidationResult.Failure("Target cannot be null");

        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(target.Target))
            result.Errors.Add("Target name cannot be empty");

        if (string.IsNullOrWhiteSpace(target.Address))
        {
            result.Errors.Add("Target address cannot be empty");
            return result;
        }

        if (!Uri.TryCreate(target.Address, UriKind.Absolute, out var uri) 
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            result.Errors.Add($"Target address '{target.Address}' is invalid. Address must start with http:// or https://");
        }

        return result;
    }
}