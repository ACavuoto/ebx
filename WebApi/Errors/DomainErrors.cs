using FluentResults;

namespace Ebx.Test.WebApi.Errors
{
    public class DomainErrors
    {
        public static class RetrieveAuthorsResponse
        {
            public static Error NotFound => new("The repository for the given owner cannot be found.");

            public static Error RateLimitExceeded => new("RateLimit exceeded, try again later");

            public static Error ServerError => new("Server error");
        }
    }
}