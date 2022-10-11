namespace LogicPublisher;

public static class TriggerRefreshPipeline
{
    [FunctionName("TriggerRefreshPipeline")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("Pipeline Execution received an HTTP trigger.");
        var settings = new AppSettings();
        try
        {
            if (settings.ReadSettings())
            {
                var result = await PipelineFunctions.TriggerPipelineExecution(settings, log);
                log.LogInformation($"Pipeline Execution Results: {settings.AzDoOrganization}/{settings.AzDoProject}/{settings.RefreshPipelineName}: {result}");
                return result;
            }
            log.LogError("Pipeline Execution Error: Error reading configuration!");
            return new BadRequestResult();
        }
        catch (Exception ex)
        {
            var errorMsg = $"Pipeline Execution Error: {settings.AzDoOrganization}/{settings.AzDoProject}/{settings.RefreshPipelineName}: {ex.Message}";
            log.LogError(errorMsg);
            return new BadRequestResult();
        }
    }
}
