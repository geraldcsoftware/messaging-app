using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MessagingApp.Tests.Utilities.Authentication;

public class TestAuthenticationSchemeHandler : AuthenticationHandler<TestAuthenticationSchemeOptions>
{
    public TestAuthenticationSchemeHandler(IOptionsMonitor<TestAuthenticationSchemeOptions> options,
                                           ILoggerFactory logger,
                                           UrlEncoder encoder,
                                           ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = Options.TestClaims;
        if (claims.All(c => c.Type != ClaimTypes.Name))
        {
            claims.Add(new Claim(ClaimTypes.Name, "TestUser"));
        }

        var identity = new ClaimsIdentity(claims, TestAuthenticationScheme.AuthenticationType);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestAuthenticationScheme.AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}