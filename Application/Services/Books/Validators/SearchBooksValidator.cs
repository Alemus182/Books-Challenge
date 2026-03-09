using Application.Services.Books.Queries;
using FluentValidation;

namespace Application.Services.Books.Validators
{
    public class SearchBooksValidator : AbstractValidator<SearchBooksRequest>
    {
        public SearchBooksValidator()
        {
            RuleFor(x => x.Query)
                .NotEmpty().WithMessage("Query cannot be empty.")
                .MaximumLength(500).WithMessage("Query must not exceed 500 characters.");
        }
    }
}
