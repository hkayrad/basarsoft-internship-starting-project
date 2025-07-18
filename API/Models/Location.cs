using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Location : IValidatableObject
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Wkt { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Id <= 0)
            yield return new ValidationResult("Id must be greater than zero.", [nameof(Id)]);

        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("Name cannot be null or empty.", [nameof(Name)]);
        else if (Name.Length > 100)
            yield return new ValidationResult("Name cannot exceed 100 characters.", [nameof(Name)]);

        if (string.IsNullOrWhiteSpace(Wkt))
            yield return new ValidationResult("WKT cannot be null or empty.", [nameof(Wkt)]);
    }
}
