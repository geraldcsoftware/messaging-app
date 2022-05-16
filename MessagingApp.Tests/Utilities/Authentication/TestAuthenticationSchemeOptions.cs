using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace MessagingApp.Tests.Utilities.Authentication;

public class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public ICollection<Claim> TestClaims { get; set; } = new List<Claim>();
}