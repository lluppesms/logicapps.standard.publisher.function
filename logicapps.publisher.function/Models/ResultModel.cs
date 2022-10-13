namespace LogicPublisher.Models;

public class ResultModel
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public ResultModel()
    {
        Success = false;
        Message = string.Empty;
    }
    public ResultModel(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}

public class TriggerPipelineResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string BuildId { get; set; }
    public string PipelineURL { get; set; }

    public TriggerPipelineResult()
    {
        Success = false;
        Message = string.Empty;
        BuildId = string.Empty;
        PipelineURL = string.Empty;
    }
    public TriggerPipelineResult(bool success, string message)
    {
        Success = success;
        Message = message;
        BuildId = string.Empty;
        PipelineURL = string.Empty;
    }
    public TriggerPipelineResult(bool success, string message, string buildId)
    {
        Success = success;
        Message = message;
        BuildId = buildId;
        PipelineURL = string.Empty;
    }
    public TriggerPipelineResult(bool success, string message, string buildId, string pipelineUrl)
    {
        Success = success;
        Message = message;
        BuildId = buildId;
        PipelineURL = pipelineUrl;
    }
    public TriggerPipelineResult(bool success, string message, string buildId, int pipelineId, string devOpsOrg, string devOpsProject)
    {
        Success = success;
        Message = message;
        BuildId = buildId;
        PipelineURL = pipelineId > 0 ? $"https://dev.azure.com/{devOpsOrg}/{devOpsProject}/_build/results?buildId={pipelineId}&view=results" : string.Empty;
    }
}