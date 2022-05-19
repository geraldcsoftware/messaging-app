using FluentValidation;
using MessagingApp.Api.ViewModels;

namespace MessagingApp.Api.Validators;

public class UpdateCampaignRequestValidator : AbstractValidator<UpdateCampaignRequest>
{
    public UpdateCampaignRequestValidator()
    {
        RuleFor(x => x.ClientId)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");
        RuleFor(x => x.Id)
           .NotEmpty()
           .WithName("Campaign Id")
           .WithMessage("{PropertyName} is required");
        RuleFor(x => x.Name)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");
        RuleFor(x => x.Name)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");
        RuleFor(x => x.Template)
           .NotEmpty()
           .WithName("Message template")
           .WithMessage("{PropertyName} is required");
    }
}