using Octokit;
using Octokit.Internal;

namespace LogicPublisher.Helpers;

public static class GitHubFunctions
{
    // See: https://octokitnet.readthedocs.io/en/latest/
    // See: https://docs.github.com/en/rest/pulls/pulls#about-the-pulls-api

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
                log.LogInformation($"Found Pull Request {pr.Number} : {pr.Title}");
                var pullRequest = new GitHubPullRequest(pr.Number, pr.Title, settings.GitHubUserName, settings.GitHubRepoName);
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
                pullRequests.Add(new GitHubPullRequest(item.Number, item.Title, settings.GitHubUserName, settings.GitHubRepoName));
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
    public static async Task<PullRequestMerge> ApprovePullRequest(int pullRequestId, AppSettings settings, ILogger log)
    {
        var pullRequests = new List<GitHubPullRequest>();
        try
        {
            var client = GetGitHubClient(settings, log);
            if (client == null) return null;
            var repo = await GetRepo(client, settings, log);
            if (repo == null) return null;

            var pullRequest = await client.PullRequest.Get(repo.Id, pullRequestId);
            if (pullRequest == null) return null;

            log.LogInformation($"Found Pull Request {pullRequest.Number} : {pullRequest.Title}");
            var mergeComment = "Automated Merge";
            var mpr = new MergePullRequest() { CommitTitle = mergeComment, MergeMethod = PullRequestMergeMethod.Squash };
            var mergeResult = await client.PullRequest.Merge(repo.Id, pullRequest.Number, mpr);
            if (mergeResult.Merged)
            {
                var head = pullRequest.Head;
                var thisBranch = await client.Repository.Branch.Get(repo.Id, pullRequest.Head.Ref);
                log.LogInformation($"Pull Request is using branch {thisBranch.Name}");
                if (thisBranch != null)
                {
                    var headPath = $"heads/{thisBranch.Name}";
                    log.LogInformation($"Deleting branch {headPath}");
                    try
                    {
                       await client.Git.Reference.Delete(repo.Id, $"{headPath}");
                       log.LogInformation($"Branch {thisBranch.Name} was deleted!");
                    }
                    catch (Exception ex)
                    {
                        var errorMsg = $"Error deleting branch heads/{thisBranch.Name}: {ex.Message}";
                        log.LogError(errorMsg);
                    }
                }
            }
            else
            {
                log.LogInformation($"Pull Request {pullRequest.Id} merge failed!");
            }
            return mergeResult;
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error approving pull request {pullRequestId}: {ex.Message}";
            log.LogError(errorMsg);
            return null;
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
