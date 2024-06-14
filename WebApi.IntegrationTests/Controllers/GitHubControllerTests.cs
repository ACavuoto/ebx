using AutoFixture;
using Ebx.Test.WebApi.IntegrationTests.Mocks;
using Ebx.Test.WebApi.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Octokit;
using System.Net;

namespace Ebx.Test.WebApi.IntegrationTests.Controllers;

public class GitHubControllerTests
{
    private readonly HttpClient _client;
    private readonly string _authorsApiUrl = " /api/v1/{0}/{1}/contributors";
    private readonly Fixture _fixture = new();

    public GitHubControllerTests()
    {
        var application = new WebApplicationFactory<Program>();
        _client = application.CreateClient();
    }

    [Theory]
    [InlineData("microsoft", "dotnet")]
    [InlineData("OpenStickCommunity", "GP2040-CE")]
    public async Task ReturnsAuthorsOfTheLast100Commits_WhenRepoAndOwnerAreValid(string owner, string repo)
    {
        var requestUrl = string.Format(_authorsApiUrl, owner, repo);

        var response = await _client.GetAsync(requestUrl);

        response.Should().HaveStatusCode(HttpStatusCode.OK);
        var content = await GetContent<RetrieveAuthorsResponse>(response);
        content.Authors.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task ReturnsNotFound_WhenRepoAndOwnerAreValid()
    {
        var owner = _fixture.Create<string>();
        var repo = _fixture.Create<string>();

        var requestUrl = string.Format(_authorsApiUrl, owner, repo);

        var response = await _client.GetAsync(requestUrl);

        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(" ", "dotnet")]
    [InlineData("OpenStickCommunity", " ")]
    [InlineData(" ", " ")]
    public async Task ReturnsBadRequest_WhenValidationFails(string owner, string repo)
    {
        var requestUrl = string.Format(_authorsApiUrl, owner, repo);

        var response = await _client.GetAsync(requestUrl);

        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReturnsTooManyRequests_WhenRateLimitExceededExceptionOccurs()
    {
        var gitHubClient = Substitute.For<IGitHubClient>();

        var mockedReponse = _fixture.Create<MockedResponse>();
        gitHubClient.Repository.Commit.GetAll(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ApiOptions>())
            .Throws(new RateLimitExceededException(mockedReponse));

        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IGitHubClient>();
                    services.AddSingleton(gitHubClient);
                });
            });
        var client = application.CreateClient();

        var owner = _fixture.Create<string>();
        var repo = _fixture.Create<string>();

        var requestUrl = string.Format(_authorsApiUrl, owner, repo);

        var response = await client.GetAsync(requestUrl);

        response.Should().HaveStatusCode(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task ReturnsServerError_WhenExceptionOccurs()
    {
        var gitHubClient = Substitute.For<IGitHubClient>();
        gitHubClient.Repository.Commit.GetAll(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ApiOptions>()).Throws(new Exception("Something unexpected happened"));

        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IGitHubClient>();
                    services.AddSingleton(gitHubClient);
                });
            });
        var client = application.CreateClient();

        var owner = _fixture.Create<string>();
        var repo = _fixture.Create<string>();

        var requestUrl = string.Format(_authorsApiUrl, owner, repo);

        var response = await client.GetAsync(requestUrl);

        response.Should().HaveStatusCode(HttpStatusCode.InternalServerError);
    }

    private static async Task<T> GetContent<T>(HttpResponseMessage response)
    {
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseJson)!;
    }
}