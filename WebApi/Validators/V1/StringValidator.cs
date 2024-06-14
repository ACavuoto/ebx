using FluentValidation;

namespace Ebx.Test.WebApi.Validators.V1;

public class StringValidator : AbstractValidator<string>
{
    public StringValidator()
    {
        RuleFor(x => x).NotEmpty()
         .WithMessage(x => $"The parameter '{x}' cannot be empty");
    }
}