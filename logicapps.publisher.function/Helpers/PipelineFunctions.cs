using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace LogicPublisher;

public static class PipelineFunctions
{
    public static async Task<IActionResult> RunPipeline(AppSettings settings, ILogger log)
    {
        log.LogInformation($"Starting PipelineExecution.TriggerPipeline for {settings.AzDoOrganization}/{settings.AzDoProject}/{settings.RefreshPipelineName}");
        try
        {
            (var success, var message, var project, var connection) = await GetProject(settings.AzDoOrganization, settings.AzDoPatToken, settings.AzDoProject, log);
            if (!success) { return new BadRequestResult(); }

            (success, message, var pipeline, var buildClient) = await GetPipeline(connection, project, settings.RefreshPipelineName, log);
            if (!success) { return new BadRequestResult(); }

            (success, message) = await TriggerPipeline(project, buildClient, pipeline, log);
            if (!success) { return new BadRequestResult(); }

            return new OkObjectResult(message);
        }
        catch (Exception ex)
        {
            var errorMsg = ex.Message;
            log.LogError(errorMsg);
            return new BadRequestResult();
        }
    }

    public static async Task<(bool result, string message, TeamProject project, VssConnection connection)> GetProject(string organization, string accessToken, string projectName, ILogger log)
    {
        var message = string.Empty;
        try
        {
            var collectionUri = new Uri($"https://dev.azure.com/{organization}");

            var creds = new VssBasicCredential(string.Empty, accessToken);
            var connection = new VssConnection(collectionUri, creds);
            if (connection == null)
            {
                message = $"Error connecting to {collectionUri}!";
                log.LogError(message);
                return (false, message, null, null);
            }

            var projectClient = connection.GetClient<ProjectHttpClient>();

            var project = await projectClient.GetProject(projectName);
            if (project == null)
            {
                message = $"Error getting project {projectName}!";
                log.LogError(message);
                return (false, message, null, null);
            }

            message = $"Project {projectName} retrieved!";
            return (true, message, project, connection);
        }
        catch (Exception ex)
        {
            message = ex.Message;
            log.LogError(message);
            return (false, message, null, null);
        }
    }

    public static async Task<(bool result, string message, List<BuildDefinitionReference> pipelineList, BuildHttpClient buildClient)> GetPipelineList(VssConnection connection, TeamProject project, ILogger log)
    {
        var message = string.Empty;
        try
        {
            var buildClient = await connection.GetClientAsync<BuildHttpClient>();
            var pipelineList = await buildClient.GetDefinitionsAsync(project.Name);
            if (pipelineList == null || !pipelineList.Any())
            {
                message = $"Error getting pipeline list for {project.Name}!";
                log.LogError(message);
                return (false, message, null, null);
            }
            message = $"Found {pipelineList.Count} pipelines for {project.Name}!";
            return (true, message, pipelineList, buildClient);
        }
        catch (Exception ex)
        {
            message = ex.Message;
            log.LogError(message);
            return (false, message, null, null);
        }
    }

    public static async Task<(bool result, string message, BuildDefinition pipeline, BuildHttpClient buildClient)> GetPipeline(VssConnection connection, TeamProject project, string pipelineName, ILogger log)
    {
        var message = string.Empty;
        try
        {
            var buildClient = await connection.GetClientAsync<BuildHttpClient>();
            var pipelineList = await buildClient.GetDefinitionsAsync(project.Name);
            if (pipelineList == null || !pipelineList.Any())
            {
                message = $"Error getting pipelines for {project.Name}!";
                log.LogError(message);
                return (false, message, null, null);
            }

            var def = pipelineList.Where(d => d.Name == pipelineName).FirstOrDefault();
            if (def == null)
            {
                message = $"Error finding pipeline {pipelineName} in list!";
                log.LogError(message);
                return (false, message, null, null);
            }

            var definition = await buildClient.GetDefinitionAsync(project.Name, def.Id);
            if (definition == null)
            {
                message = $"Error getting pipeline definition for {pipelineName}!";
                log.LogError(message);
                return (false, message, null, null);
            }
            message = $"Found pipeline {pipelineName}!";
            return (true, message, definition, buildClient);
        }
        catch (Exception ex)
        {
            message = ex.Message;
            log.LogError(message);
            return (false, message, null, null);
        }
    }

    public static async Task<(bool result, string message)> TriggerPipeline(TeamProjectReference project, BuildHttpClient buildClient, BuildDefinition pipeline, ILogger log)
    {
        var message = string.Empty;
        try
        {
            var build = new Build
            {
                Definition = pipeline,
                Project = project
            };

            var response = await buildClient.QueueBuildAsync(build, ignoreWarnings: true);
            if (response == null)
            {
                message = $"Error submitting Pipeline Trigger for {pipeline.Name}!";
                log.LogError(message);
                return (false, message);
            }

            message = $"Pipeline Trigger returned Build Number: {response.BuildNumber}";

            return (true, message);
        }
        catch (Exception ex)
        {
            message = ex.Message;
            log.LogError(message);
            return (false, message);
        }
    }
}
