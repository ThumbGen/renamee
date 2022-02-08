using FluentValidation;
using renamee.Shared.Helpers;

namespace renamee.Shared.Validators
{
    public class FormatPatternValidator : AbstractValidator<string>
    {
        public FormatPatternValidator()
        {
            RuleFor(format => FormatParser.Validate(format));
        }
    }
}
