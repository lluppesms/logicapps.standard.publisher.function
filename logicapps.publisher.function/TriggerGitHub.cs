namespace LogicPublisher;

public static class TriggerGitHubProxy
{
    [FunctionName("TriggerGitHubGetPullRequests")]
    public static async Task<IActionResult> GetPullRequests([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("GitHub GetPullRequests received an HTTP trigger.");
        var settings = new AppSettings();
        if (settings.IsValid())
        {
            var pullRequests = await GitHubFunctions.GetPullRequests(settings, log);
            if (pullRequests != null) {
                log.LogInformation($"GitHub GetPullRequests Found {pullRequests.Count} requests!");
                return new OkObjectResult(pullRequests);
            }
        }
        return new BadRequestResult();
    }

    [FunctionName("TriggerGitHubGetFirstPullRequest")]
    public static async Task<IActionResult> GetFirstPullRequest([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("GitHub UserInfo received an HTTP trigger.");
        var settings = new AppSettings();
        if (settings.IsValid())
        {
            var pr = await GitHubFunctions.GetFirstPullRequest(settings, log);
            if (pr != null)
            {
                log.LogInformation($"GitHub First Pull Request Results: {pr.Id} : {pr.Title}");
                return new OkObjectResult(pr);
            }
        }
        return new BadRequestResult();
    }

    [FunctionName("TriggerGitHubGetUserInfo")]
    public static async Task<IActionResult> GetUserInfo([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("GitHub UserInfo received an HTTP trigger.");
        var settings = new AppSettings();
        if (settings.IsValid())
        {
            var result = await GitHubFunctions.GetUserInfo(settings, log);
            log.LogInformation($"GitHub UserInfo Results: {result}");
            return new OkObjectResult(result);
        }
        return new BadRequestResult();
    }
}
