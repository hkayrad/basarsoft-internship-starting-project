
namespace API.Models.DTOs.Feature;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using API.Helpers.Resources;

public class BaseFeatureDto : IValidatableObject
{
    public required string Name { get; set; }

    public required string Wkt { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult(FeatureDtoResourceHelper.GetString("NameRequired"), [nameof(Name)]);
        else
        {
            if (Name.Length > 100 || Name.Length < 3)
                yield return new ValidationResult(FeatureDtoResourceHelper.GetString("NameLengthInvalid"), [nameof(Name)]);

            if (Regex.IsMatch(Name, @"[^a-zA-Z0-9 ,'()/-]"))
                yield return new ValidationResult(FeatureDtoResourceHelper.GetString("NameCharacterInvalid"), [nameof(Name)]);
        }

        if (string.IsNullOrWhiteSpace(Wkt))
            yield return new ValidationResult(FeatureDtoResourceHelper.GetString("WktRequired"), [nameof(Wkt)]);
        else
            if (Regex.IsMatch(Wkt, @"[^A-Z0-9() ,-.]"))
            yield return new ValidationResult(FeatureDtoResourceHelper.GetString("WktInvalidFormat"), [nameof(Wkt)]);
    }
}
