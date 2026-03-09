using Application.Services.Auth.Commands;
using FluentValidation;

namespace Application.Services.Auth.Validators
{
    public class SingInValidator : AbstractValidator<SingInRequest>
    {
        public SingInValidator()
        {
            RuleFor(e => e.userName).NotEmpty().EmailAddress();
            RuleFor(e => e.password).NotEmpty().MinimumLength(8);
        }
    }
}
