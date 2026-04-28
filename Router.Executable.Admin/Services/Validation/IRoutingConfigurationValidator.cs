using Router.Executable.Admin.Models;

namespace Router.Executable.Admin.Services.Validation;

public interface IRoutingConfigurationValidator
{
    ValidationResult Validate();
}