using Ebx.Test.WebApi.Errors;
using Ebx.Test.WebApi.Requests;
using Ebx.Test.WebApi.Responses;
using FluentResults;
using MediatR;
using Octokit;

namespace Ebx.Test.WebApi.RequestHandlers
{
    public class RetrieveAuthorsHandler(IGitHubClient client, ILogger<RetrieveAuthorsHandler> logger) : IRequestHandler<RetrieveAuthorsRequest, Result<RetrieveAuthorsResponse>>
    {
        public async Task<Result<RetrieveAuthorsResponse>> Handle(RetrieveAuthorsRequest request, CancellationToken cancellationToken)
        {
            return await GetAuthorsFromCommits(request);
        }

        private async Task<Result<RetrieveAuthorsResponse>> GetAuthorsFromCommits(RetrieveAuthorsRequest retrieveAuthorsRequest)
        {
            var authors = new HashSet<string>();

            //TODO: move this config to appsettings.json and inject it
            var options = new ApiOptions
            {
                PageSize = 100,
                PageCount = 1,
                StartPage = 1
            };

            try
            {
                while (authors.Count < retrieveAuthorsRequest.AuthorsToTake)
                {
                    var commits = await client.Repository.Commit.GetAll(retrieveAuthorsRequest.Owner, retrieveAuthorsRequest.Repo, options);

                    if (commits.Count == 0)
                    {
                        break;
                    }

                    foreach (var commit in commits)
                    {
                        if (commit.Commit.Author != null)
                        {
                            authors.Add(commit.Commit.Author.Name);
                        }
                        else if (commit.Commit.Committer != null)
                        {
                            authors.Add(commit.Commit.Committer.Name);
                        }

                        if (authors.Count == retrieveAuthorsRequest.AuthorsToTake)
                        {
                            break;
                        }
                    }

                    options.StartPage++;
                }

                return new RetrieveAuthorsResponse([.. authors]);
            }
            catch (NotFoundException)
            {
                logger.LogInformation("The repository '{owner}/{repo}' cannot be found.", retrieveAuthorsRequest.Owner, retrieveAuthorsRequest.Repo);
                return Result.Fail(DomainErrors.RetrieveAuthorsResponse.NotFound);
            }
            catch (RateLimitExceededException ex)
            {
                //TODO: Implement retry mechanism
                logger.LogError(ex, "Unable to complete GetAuthorsFromCommits: RateLimitExceeded");
                return Result.Fail(DomainErrors.RetrieveAuthorsResponse.RateLimitExceeded);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to complete GetAuthorsFromCommits: Server error");
                return Result.Fail(DomainErrors.RetrieveAuthorsResponse.ServerError);
            }
        }
    }
}