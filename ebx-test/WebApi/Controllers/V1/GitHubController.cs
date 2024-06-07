using Ebx.Test.WebApi.Errors;
using Ebx.Test.WebApi.Requests;
using Ebx.Test.WebApi.Responses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace Ebx.Test.WebApi.Controllers.V1
{
    [ApiController]
    [Route("/api/v1")]
    public class GitHubController(IMediator mediator, ILogger<GitHubController> logger, IValidator<string> stringValidator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<GitHubController> _logger = logger;

        [HttpGet("{owner}/{repo}/contributors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RetrieveAuthorsResponse>> GetContributors(string owner, string repo)
        {
            var ownerValidationResult = stringValidator.Validate(owner);
            var repoValidationResult = stringValidator.Validate(repo);

            if (!ownerValidationResult.IsValid || !repoValidationResult.IsValid)
            {
                var validationErrors = ownerValidationResult.Errors.Concat(repoValidationResult.Errors);

                _logger.LogInformation("Validation failed for owner '{owner}' and repo '{repo}'", owner, repo);
                return new BadRequestObjectResult(validationErrors);
            }

            var retrieveAuthorsRequest = new RetrieveAuthorsRequest(owner, repo);

            var result = await _mediator.Send(retrieveAuthorsRequest);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.HasError(x => x.Message == DomainErrors.RetrieveAuthorsResponse.NotFound.Message))
            {
                return NotFound();
            }

            _logger.LogError("GetContributors failed with errors {errors}", result.Errors);

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}