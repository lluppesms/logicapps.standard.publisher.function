using Octokit;
using Octokit.Internal;

namespace LogicPublisher.Helpers;

public static class GitHubFunctions
{
    public static async Task<string> GetPullRequests(AppSettings settings, ILogger log)
    {
        // See: https://octokitnet.readthedocs.io/en/latest/
        var message = string.Empty;
        try
        {
            var tokenAuth = new Credentials(settings.GitHubPatToken); // This can be a PAT or an OAuth token.
            var credentials = new InMemoryCredentialStore(tokenAuth);
            var client = new GitHubClient(new ProductHeaderValue(Constants.GitHubProductHeader), credentials);
            
            var user = await client.User.Get(settings.GitHubUserName);
            log.LogInformation($"{user.Followers} folks love {settings.GitHubUserName}!");
            log.LogInformation($"{settings.GitHubUserName} Email is {user.Email}");

            var repo = await client.Repository.Get(settings.GitHubUserName, settings.GitHubRepoName);
            log.LogInformation($"{settings.GitHubUserName}/{settings.GitHubRepoName} has {repo.StargazersCount} stars and {repo.ForksCount} forks.");

            var prList = await client.PullRequest.GetAllForRepository(repo.Id);
            log.LogInformation($"Found {prList.Count} pull requests.");
            if (prList.Count > 0)
            {
                var pr = prList.FirstOrDefault();
                message = "First PR:" + 
                    $" Id: {pr.Id}" +
                    $" Title: {pr.Title}";
            }
            else
            {
                message = $"No pull requests found for {settings.GitHubRepoName}";
            }
            return message;
        }
        catch (Exception ex)
        {
            var errorMsg = ex.Message;
            log.LogError(errorMsg);
            return "Failed!";
            // return new BadRequestResult();
        }
    }
}
