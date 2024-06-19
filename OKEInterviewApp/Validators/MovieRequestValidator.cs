using FluentValidation;

namespace OKEInterviewApp.Validators;

public class MovieRequestValidator : AbstractValidator<string>
{
    public MovieRequestValidator()
    {
        RuleFor(source => source).NotEmpty().WithMessage("Source must be specified.");
    }
}
