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
    }
    public bool IsValid()
    {
        return (!string.IsNullOrEmpty(AzDoOrganization) &&
        !string.IsNullOrEmpty(AzDoPatToken) &&
        !string.IsNullOrEmpty(AzDoProject) &&
        !string.IsNullOrEmpty(RefreshPipelineName) &&
        !string.IsNullOrEmpty(GitHubPatToken) &&
        !string.IsNullOrEmpty(GitHubUserName) &&
        !string.IsNullOrEmpty(GitHubRepoName));
    }
}
