namespace LogicPublisher;

public static class TriggerGitHubProxy
{
    [FunctionName("TriggerGitHubApprovePullRequest")]
    public static async Task<IActionResult> ApprovePullRequest([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("GitHub GetPullRequests received an HTTP trigger.");
        var settings = new AppSettings();
        if (settings.IsValid())
        {
            var pullRequestId = Common.ParseIntegerFromRequest(req, "id");
            if (pullRequestId > 0) {
                var mergeRequest = await GitHubFunctions.ApprovePullRequest(pullRequestId, settings, log);
                if (mergeRequest != null) {
                    log.LogInformation($"GitHub ApprovePullRequest returned Merged: {mergeRequest.Merged} Message: {mergeRequest.Message}!");
                    return new OkObjectResult(mergeRequest);
                }
            }
        }
        return new BadRequestResult();
    }

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
                log.LogInformation($"GitHub First Pull Request Results: {pr.Number} : {pr.Title}");
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
