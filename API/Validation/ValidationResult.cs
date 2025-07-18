using System;

namespace API.Validation;

public class ValidationResult
{
    public required bool IsValid { get; set; }

    public string? ErrorMessage { get; set; }

    public static ValidationResult Valid()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Invalid(string errorMessage)
    {
        return new ValidationResult { IsValid = false, ErrorMessage = errorMessage };
    }
}
