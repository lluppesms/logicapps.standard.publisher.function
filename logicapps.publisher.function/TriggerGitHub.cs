namespace LogicPublisher;

public static class TriggerGitHubProxy
{
    [FunctionName("TriggerGitHubProxy")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("GitHub Proxy received an HTTP trigger.");
        var settings = new AppSettings();
        try
        {
            if (settings.ReadSettings())
            {
                var result = await GitHubFunctions.GetPullRequests(settings, log);
                log.LogInformation($"GitHub Proxy Results: {settings.GitHubUserName}/{settings.GitHubRepoName}: {result}");
                return new OkObjectResult(result);
            }
            log.LogError("GitHub Proxy Error: Error reading configuration!");
            return new BadRequestResult();
        }
        catch (Exception ex)
        {
            var errorMsg = $"GitHub Proxy Error: {settings.GitHubUserName}/{settings.GitHubRepoName}: {ex.Message}";
            log.LogError(errorMsg);
            return new BadRequestResult();
        }
    }
}
