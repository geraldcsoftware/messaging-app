using FluentValidation;
using MessagingApp.Api.ViewModels;

namespace MessagingApp.Api.Validators;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}