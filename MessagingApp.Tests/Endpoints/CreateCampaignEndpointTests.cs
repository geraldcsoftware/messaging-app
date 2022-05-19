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

using System.Net;

using Xunit.Abstractions;

public class CreateCampaignEndpointTests : IClassFixture<ApplicationFactory>
{
    private readonly WebApplicationFactory<CreateCampaignEndpoint> _factory;

    private readonly ITestOutputHelper _outputHelper;

    public CreateCampaignEndpointTests(ApplicationFactory factory,
                                       ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _outputHelper = outputHelper;
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

        var response = await httpClient.PostAsync("/api/clients/101/campaigns", requestContent);
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

    [Theory]
    [InlineData("", "Some Value")]
    [InlineData(null, "Some Value")]
    [InlineData(" ", "Some Value")]
    [InlineData("Some Value", "")]
    [InlineData("Some Value", null)]
    [InlineData("Some Value", " ")]
    public async Task CreateCampaign_WhenInvalidDataIsPassed_ShouldReturnBadRequest(string? name, string? messageTemplate)
    {
        // Arrange
        var idGenerator = new Mock<IIdGenerator>();
        idGenerator.Setup(x => x.NewId()).Returns(Guid.NewGuid().ToString());

        var httpClient = _factory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"{TestAuthenticationScheme.AuthenticationScheme} SomeToken");
        var request = $$"""
                      {
                        "name": "{{name}}",
                        "messageTemplate": "{{messageTemplate}}",
                        "placeHolders":[]
                      }
                      """;
        var requestContent = new StringContent(request, Encoding.UTF8, "application/json");

        // Act

        var response = await httpClient.PostAsync("/api/clients/101/campaigns", requestContent);
        var responseContent = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine("[Status code - {0}]: {1}", response.StatusCode,responseContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}