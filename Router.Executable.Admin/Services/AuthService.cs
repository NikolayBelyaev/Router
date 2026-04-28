using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Router.Executable.Admin.Configuration;

namespace Router.Executable.Admin.Services;

public class AuthService(IOptions<UserCredentialsConfig> userCredentialsOptions)
{
    private readonly UserCredentialsConfig _credentialsConfig = userCredentialsOptions.Value;

    public bool ValidateCredentials(string login, string password)
    {
        var configLogin = _credentialsConfig.Login;
        var configPassword = _credentialsConfig.Password;
        return login == configLogin && password == configPassword;
    }

    public ClaimsPrincipal CreatePrincipal(string login, string credentialsHash)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, login),
            new(ClaimTypes.Role, "Admin"),
            new("CredentialsHash", credentialsHash)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        return new ClaimsPrincipal(identity);
    }
    
    public string GetCredentialsHash()
    {
        var login = _credentialsConfig.Login;
        var password = _credentialsConfig.Password;
        return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes($"{login}:{password}")));
    }
}