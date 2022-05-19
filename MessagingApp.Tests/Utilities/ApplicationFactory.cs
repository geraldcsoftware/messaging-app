using MessagingApp.Api.Endpoints;
using MessagingApp.Tests.Utilities.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace MessagingApp.Tests.Utilities;

public class ApplicationFactory : WebApplicationFactory<CreateCampaignEndpoint>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.AddInMemoryDatabase();
            services.AddAuthentication(TestAuthenticationScheme.AuthenticationScheme)
                    .AddScheme<TestAuthenticationSchemeOptions,
                         TestAuthenticationSchemeHandler>(TestAuthenticationScheme.AuthenticationScheme,
                                                          options =>
                                                          {
                                                              options.TestClaims.Add(new("ClientId", "101"));
                                                              options.TestClaims.Add(new("ApplicationId", "Test"));
                                                          });
        });
    }
}