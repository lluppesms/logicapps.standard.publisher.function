namespace LogicPublisher;

public static class TriggerRefreshPipeline
{
    [FunctionName("TriggerRefreshPipeline")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("Pipeline Execution received an HTTP trigger.");
        var settings = new AppSettings();
        try
        {
            if (settings.IsValid())
            {
                var result = await PipelineFunctions.RunPipeline(settings, log);
                log.LogInformation($"Pipeline Execution Results: {settings.AzDoOrganization}/{settings.AzDoProject}/{settings.RefreshPipelineName}: {result}");
                return new OkObjectResult(result);
            }
            log.LogError("Pipeline Execution Error: Error reading configuration!");
            return new OkObjectResult(new ResultModel(false, "Invalid configuration!"));
        }
        catch (Exception ex)
        {
            var errorMsg = $"Pipeline Execution Error: {settings.AzDoOrganization}/{settings.AzDoProject}/{settings.RefreshPipelineName}: {ex.Message}";
            log.LogError(errorMsg);
            return new BadRequestResult();
        }
    }
}
