using Ebx.Test.WebApi.Responses;
using FluentResults;
using MediatR;

namespace Ebx.Test.WebApi.Requests
{
    public record RetrieveAuthorsRequest(string Owner, string Repo, int AuthorsToTake = 100) : IRequest<Result<RetrieveAuthorsResponse>>;
}