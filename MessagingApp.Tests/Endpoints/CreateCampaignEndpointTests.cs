using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using MessagingApp.Api.Endpoints;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure;
using MessagingApp.Infrastructure.Models;
using MessagingApp.Tests.Utilities;
using MessagingApp.Tests.Utilities.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MessagingApp.Tests.Endpoints;

public class CreateCampaignEndpointTests : IClassFixture<WebApplicationFactory<CreateCampaignEndpoint>>
{
    private readonly WebApplicationFactory<CreateCampaignEndpoint> _factory;

    public CreateCampaignEndpointTests(WebApplicationFactory<CreateCampaignEndpoint> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateCampaign_ShouldBeSuccessful()
    {
        // Arrange
        var idGenerator = new Mock<IIdGenerator>();
        idGenerator.Setup(x => x.NewId()).Returns(Guid.NewGuid().ToString());

        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddInMemoryDatabase();
                services.AddAuthentication(TestAuthenticationScheme.AuthenticationScheme)
                        .AddScheme<TestAuthenticationSchemeOptions,
                             TestAuthenticationSchemeHandler>(TestAuthenticationScheme.AuthenticationScheme,
                                                              options =>
                                                              {
                                                                  options.TestClaims.Add(new("ClientId", "101"));
                                                              });

                using var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Clients.Add(new Client { Id = "101", Name = "Test Client" });
                dbContext.SaveChanges();
            });
        });

        var httpClient = factory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Authorization",
                                             $"{TestAuthenticationScheme.AuthenticationScheme} SomeToken");
        const string request = """"
        {
            "name": "Test Campaign",
            "messageTemplate": "This is a test message template",
            "placeHolders":[ "FirstName", "LastName"]
        }
        """";
        var requestContent = new StringContent(request, Encoding.UTF8, "application/json");

        // Act

        var response = await httpClient.PostAsync($"/api/clients/101/campaigns", requestContent);
        var result = response.IsSuccessStatusCode
                         ? await response.Content.ReadFromJsonAsync<CampaignViewModel>()
                         : null;

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeNullOrEmpty();
        result.Name.Should().Be("Test Campaign");
        result.MessageTemplate.Should().Be("This is a test message template");
        result.ClientId.Should().Be("101");
    }
}