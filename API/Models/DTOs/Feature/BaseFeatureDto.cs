
namespace API.Models.DTOs.Feature;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class BaseFeatureDto : IValidatableObject
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

            if (Regex.IsMatch(Name, @"[^a-zA-Z0-9 ,'()/-]"))
                yield return new ValidationResult("Name cannot contain special characters.", [nameof(Name)]);
        }

        if (string.IsNullOrWhiteSpace(Wkt))
            yield return new ValidationResult("WKT cannot be null or empty.", [nameof(Wkt)]);
        else
            if (Regex.IsMatch(Wkt, @"[^A-Z0-9() ,-.]"))
            yield return new ValidationResult("Please format the WKT correctly.", [nameof(Wkt)]);
    }
}
