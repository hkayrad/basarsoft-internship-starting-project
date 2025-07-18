
namespace API.Models.Location;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
// using Validation;

public class BaseLocationDto : IValidatableObject
{
    public required string Name { get; set; }

    public required string Wkt { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("Name cannot be null or empty.", [nameof(Name)]);
        else
        {
            if (Name.Length > 100 || Name.Length < 3)
                yield return new ValidationResult("Name must be between 3 and 100 characters.", [nameof(Name)]);

            if (Regex.IsMatch(Name, @"[^a-zA-Z0-9 ]"))
                yield return new ValidationResult("Name cannot contain special characters.", [nameof(Name)]);
        }

        if (string.IsNullOrWhiteSpace(Wkt))
            yield return new ValidationResult("WKT cannot be null or empty.", [nameof(Wkt)]);
        else
            if (Regex.IsMatch(Wkt, @"[^A-Z0-9() ,]"))
            yield return new ValidationResult("Please format the WKT correctly.", [nameof(Wkt)]);
    }

    // public IEnumerable<ValidationResult> IsValid()
    // {
    //     if (string.IsNullOrWhiteSpace(Name))
    //         yield return ValidationResult.Invalid("Name cannot be null or empty.");
    //     else
    //     {
    //         if (Name.Length > 100 || Name.Length < 3)
    //             yield return ValidationResult.Invalid("Name must be between 3 and 100 characters.");

    //         if (Regex.IsMatch(Name, @"[^a-zA-Z0-9 ]"))
    //             yield return ValidationResult.Invalid("Name cannot contain special characters.");
    //     }

    //     if (string.IsNullOrWhiteSpace(Wkt))
    //         yield return ValidationResult.Invalid("WKT cannot be null or empty.");
    //     else
    //         if (Regex.IsMatch(Wkt, @"[^A-Z0-9() ,]"))
    //         yield return ValidationResult.Invalid("Please format the WKT correctly.");

    //     yield return ValidationResult.Valid();
    // }
}
