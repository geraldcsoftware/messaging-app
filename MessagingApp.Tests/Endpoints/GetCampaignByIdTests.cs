using System.Net;
using System.Text.Json;
using Bogus;
using FluentAssertions;
using MessagingApp.Infrastructure;
using MessagingApp.Infrastructure.Models;
using MessagingApp.Tests.Utilities;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace MessagingApp.Tests.Endpoints;

public class GetCampaignByIdTests : IClassFixture<ApplicationFactory>
{
    private readonly ApplicationFactory _factory;

    public GetCampaignByIdTests(ApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCampaignById_WhenCampaignExists_ReturnsOkWithCampaignDetails()
    {
        // Arrange
        var clientId = Guid.NewGuid().ToString();
        var campaignId = Guid.NewGuid().ToString();

        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
                var client = new Faker<Client>()
                            .RuleFor(x => x.Id, clientId)
                            .RuleFor(x => x.Name, f => f.Person.Company.Name)
                            .RuleFor(x => x.Email, f => f.Person.Email)
                            .Generate();
                var campaign = new Faker<MessageCampaign>()
                              .RuleFor(x => x.Id, campaignId)
                              .RuleFor(x => x.Name, f => f.Lorem.Word())
                              .RuleFor(x => x.Template, f => f.Lorem.Sentence())
                              .RuleFor(x => x.Created, f => f.Date.Recent())
                              .RuleFor(x => x.ClientId, clientId)
                              .Generate();

                context.Clients.Add(client);
                context.Campaigns.Add(campaign);
                context.SaveChanges();
            });
        });

        var client = factory.CreateClient();
        var url = $"/api/clients/{clientId}/campaigns/{campaignId}";

        // Act
        var response = await client.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseContent.Should().NotBeNullOrEmpty();
        var responseJson = JsonDocument.Parse(responseContent);

        responseJson.RootElement.GetProperty("id").GetString().Should().Be(campaignId);
        responseJson.RootElement.GetProperty("name").GetString().Should().NotBeNullOrEmpty();
        responseJson.RootElement.GetProperty("messageTemplate").GetString().Should().NotBeNullOrEmpty();
        responseJson.RootElement.GetProperty("created").GetDateTime().Should().NotBe(default);
    }
    
     [Fact]
    public async Task GetCampaignById_WhenCampaignDoesntExist_ReturnsNotFound()
    {
        // Arrange
        var clientId = Guid.NewGuid().ToString();
        var campaignId = Guid.NewGuid().ToString();

        var client = _factory.CreateClient();
        var url = $"/api/clients/{clientId}/campaigns/{campaignId}";

        // Act
        var response = await client.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}