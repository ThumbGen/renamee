using FluentValidation;
using renamee.Shared.Models;

namespace renamee.Shared.Validators
{
    public class SettingsValidator: AbstractValidator<Settings>
    {
        public SettingsValidator()
        {
            RuleFor(x => x.Geocoder)
                .NotNull()
                .WithMessage("Provide Geocoder settings.")
                .SetValidator(x => new GeocoderSettingsValidator());
        }
    }

    public class GeocoderSettingsValidator: AbstractValidator<GeocoderSettings>
    {
        public GeocoderSettingsValidator()
        {
            // no rules for now
        }
    }
}
