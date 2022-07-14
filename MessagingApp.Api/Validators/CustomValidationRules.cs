using System.Text.RegularExpressions;
using FluentValidation;

namespace MessagingApp.Api.Validators;

public static partial class CustomValidationRules
{
    public static IRuleBuilderOptions<T, IReadOnlyCollection<string>> PhoneNumberList<T>(
        this IRuleBuilder<T, IReadOnlyCollection<string>> ruleBuilder)
    {
        return ruleBuilder.Must((_, list, context) =>
                           {
                               var phoneNumberRegex = PhoneNumberRegex();
                               foreach (var phoneNumber in list)
                               {
                                   if (phoneNumberRegex.IsMatch(phoneNumber))
                                   {
                                       continue;
                                   }

                                   context.MessageFormatter.AppendArgument("PhoneNumber", phoneNumber);
                                   return false;
                               }

                               return true;
                           })
                          .WithMessage("`{PhoneNumber}` is not a valid phone number");
    }

    [RegexGenerator(@"^\+?\d{10,13}$", RegexOptions.Compiled)]
    private static partial Regex PhoneNumberRegex();
}