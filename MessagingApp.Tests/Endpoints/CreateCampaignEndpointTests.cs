using System.Net.Http.Json;
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
        var request = new CreateCampaignRequest
        {
            ClientId = "101",
            Name = "Test Campaign",
            MessageTemplate = "This is a test message template",
            PlaceHolders = { "FirstName", "LastName" }
        };

        // Act

        var response = await httpClient.PostAsJsonAsync($"/api/clients/101/campaigns", request);
        var result = response.IsSuccessStatusCode
                         ? await response.Content.ReadFromJsonAsync<CampaignViewModel>()
                         : null;

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeNullOrEmpty();
        result.Name.Should().Be(request.Name);
        result.MessageTemplate.Should().Be(request.MessageTemplate);
    }
}