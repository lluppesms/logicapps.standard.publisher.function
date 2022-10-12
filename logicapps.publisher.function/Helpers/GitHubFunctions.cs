using Octokit;
using Octokit.Internal;

namespace LogicPublisher.Helpers;

public static class GitHubFunctions
{
    // See: https://octokitnet.readthedocs.io/en/latest/

    public static async Task<string> GetUserInfo(AppSettings settings, ILogger log)
    {
        var message = string.Empty;
        try
        {
            var client = GetGitHubClient(settings, log);
            if (client == null) return null;

            var user = await client.User.Get(settings.GitHubUserName);
            log.LogInformation($"{user.Followers} folks love {settings.GitHubUserName}!");
            log.LogInformation($"{settings.GitHubUserName} Email is {user.Email}");
            message = $"{settings.GitHubUserName} has {user.Followers} followers!";
            return message;
        }
        catch (Exception ex)
        {
            var errorMsg = ex.Message;
            log.LogError(errorMsg);
            return "Failed!";
        }
    }
    public static async Task<GitHubPullRequest> GetFirstPullRequest(AppSettings settings, ILogger log)
    {
        try
        {
            var client = GetGitHubClient(settings, log);
            if (client == null) return null;
            var repo = await GetRepo(client, settings, log);
            if (repo == null) return null;

            var prList = await client.PullRequest.GetAllForRepository(repo.Id);
            log.LogInformation($"Found {prList.Count} pull requests.");
            if (prList.Count > 0)
            {
                var pr = prList.FirstOrDefault();
                log.LogInformation($"Found Pull Request {pr.Id} : {pr.Title}");
                var pullRequest = new GitHubPullRequest(pr.Id, pr.Title);
                return pullRequest;
            }
            log.LogInformation($"No pull requests found for {settings.GitHubRepoName}");
            return null;
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error fetching first pull request: {ex.Message}";
            log.LogError(errorMsg);
            return null;
        }
    }
    public static async Task<List<GitHubPullRequest>> GetPullRequests(AppSettings settings, ILogger log)
    {
        var pullRequests = new List<GitHubPullRequest>();
        try
        {
            var client = GetGitHubClient(settings, log);
            if (client == null) return null;
            var repo = await GetRepo(client, settings, log);
            if (repo == null) return null;

            var prList = await client.PullRequest.GetAllForRepository(repo.Id);
            log.LogInformation($"Found {prList.Count} pull requests.");
            foreach (var item in prList)
            {
                pullRequests.Add(new GitHubPullRequest(item.Id, item.Title));
            }
            return pullRequests;
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error fetching list of pull requests: {ex.Message}";
            log.LogError(errorMsg);
            return new List<GitHubPullRequest>();
        }
    }

    private static GitHubClient GetGitHubClient(AppSettings settings, ILogger log)
    {
        try
        {
            if (string.IsNullOrEmpty(settings.GitHubPatToken)) { return null; }
            var tokenAuth = new Credentials(settings.GitHubPatToken); // This can be a PAT or an OAuth token.
            var credentials = new InMemoryCredentialStore(tokenAuth);
            var client = new GitHubClient(new ProductHeaderValue(Constants.GitHubProductHeader), credentials);
            return client;
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error getting GitHubClient: {ex.Message}";
            log.LogError(errorMsg);
            return null;
        }
    }
    private static async Task<Repository> GetRepo(GitHubClient client, AppSettings settings, ILogger log)
    {
        try
        {
            if (client == null) { return null; }
            var repo = await client.Repository.Get(settings.GitHubUserName, settings.GitHubRepoName);
            log.LogInformation($"{settings.GitHubUserName}/{settings.GitHubRepoName} has {repo.StargazersCount} stars and {repo.ForksCount} forks.");
            return repo;
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error getting Repository {settings.GitHubRepoName}: {ex.Message}";
            log.LogError(errorMsg);
            return null;
        }

    }
}
