using FluentValidation;
using MessagingApp.Api.ViewModels;
using MessagingApp.Scheduling;

namespace MessagingApp.Api.Validators;

public class ScheduleCampaignRequestValidator: AbstractValidator<ScheduleCampaignRequest>
{
    public ScheduleCampaignRequestValidator()
    {
        RuleFor(x => x.CampaignId).NotEmpty();
        RuleFor(x => x.ScheduleType).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Customers).NotEmpty()
                                 .Must(customers => customers.Count > 0)
                                 .WithMessage("You must select at least one customer")
                                 .PhoneNumberList();

        When(x => x.ScheduleType == ScheduleTypes.Instant,
            () =>
                {
                    // Validate instant schedules.
                    RuleFor(x => x.Customers)
                        .Must(val => val.Count < 10)
                        .WithMessage("Cannot create an instant message for more than 1 customer");
                });
    }
}