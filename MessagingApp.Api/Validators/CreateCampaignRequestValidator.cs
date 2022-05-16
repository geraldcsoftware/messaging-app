using FluentValidation;
using MessagingApp.Api.ViewModels;

namespace MessagingApp.Api.Validators;

public class CreateCampaignRequestValidator : AbstractValidator<CreateCampaignRequest>
{
    public CreateCampaignRequestValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");

        RuleFor(x => x.ClientId)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");

        RuleFor(x => x.MessageTemplate)
           .NotEmpty()
           .WithName("Message Template")
           .WithMessage("{PropertyName} is required");
    }
}