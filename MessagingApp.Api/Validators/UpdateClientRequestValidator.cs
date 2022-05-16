using FluentValidation;
using MessagingApp.Api.ViewModels;

namespace MessagingApp.Api.Validators;

public class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}