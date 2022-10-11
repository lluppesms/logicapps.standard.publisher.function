namespace LogicPublisher.Models;

public class AppSettings
{
    public string AzDoOrganization { get; set; }
    public string AzDoPatToken { get; set; }
    public string AzDoProject { get; set; }
    public string RefreshPipelineName { get; set; }
    public string GitHubPatToken { get; set; }
    public string GitHubUserName { get; set; }
    public string GitHubRepoName { get; set; }

    public AppSettings()
    {
    }

    public bool ReadSettings()
    {
        if (string.IsNullOrEmpty(AzDoOrganization))
        {
            AzDoOrganization = Common.GetEnvironmentVariable("AzDoOrganization");
            AzDoPatToken = Common.GetEnvironmentVariable("AzDoPatToken");
            AzDoProject = Common.GetEnvironmentVariable("AzDoProject");
            RefreshPipelineName = Common.GetEnvironmentVariable("RefreshPipelineName");
            GitHubPatToken = Common.GetEnvironmentVariable("GitHubPatToken");
            GitHubUserName = Common.GetEnvironmentVariable("GitHubUserName");
            GitHubRepoName = Common.GetEnvironmentVariable("GitHubRepoName");
        }
        return !string.IsNullOrEmpty(AzDoOrganization);
    }
}
