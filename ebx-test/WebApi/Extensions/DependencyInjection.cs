using Octokit;

namespace Ebx.Test.WebApi.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOctokitGitHubClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGitHubClient>(provider =>
            {
                var githubToken = configuration.GetSection("GitHubSettings:Token").Get<string>();
                var client = new GitHubClient(new ProductHeaderValue("ebx-test"))
                {
                    Credentials = new Credentials(githubToken)
                };
                return client;
            });

            return services;
        }
    }
}
