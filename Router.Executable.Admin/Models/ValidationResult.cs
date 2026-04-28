namespace Router.Executable.Admin.Models;

public class ValidationResult
{
    public List<string> Errors { get; } = [];
    public bool IsValid => Errors.Count == 0;

    public static ValidationResult Success() => new();
    
    public static ValidationResult Failure(params string[] errors)
    {
        var result = new ValidationResult();
        result.Errors.AddRange(errors);
        return result;
    }
    
    public void Merge(ValidationResult other) => Errors.AddRange(other.Errors);
}