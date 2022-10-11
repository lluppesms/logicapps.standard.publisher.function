namespace LogicPublisher.Models;

public class AppSettings
{
    public string AzDoOrganization { get; set; }
    public string AzDoPatToken { get; set; }
    public string AzDoProject { get; set; }
    public string RefreshPipelineName { get; set; }

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
        }
        return !string.IsNullOrEmpty(AzDoOrganization);
    }
}
