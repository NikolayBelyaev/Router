using Router.Executable.Admin.Models;

namespace Router.Executable.Admin.Services.Validation;

public interface IRoutingValidator
{
    ValidationResult Validate(List<RoutingServer> servers);
}