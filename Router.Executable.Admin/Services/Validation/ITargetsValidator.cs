using Router.Executable.Admin.Models;

namespace Router.Executable.Admin.Services.Validation;

public interface ITargetsValidator
{
    ValidationResult Validate(List<TargetViewModel> targets);
}