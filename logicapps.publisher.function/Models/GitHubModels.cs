using Microsoft.TeamFoundation.Test.WebApi;

namespace LogicPublisher.Models;

// public class GitHubPullRequests
// {
//     public List<GitHubPullRequest> Requests { get; set; }
//     public GitHubPullRequests()
//     {
//         Requests = new List<GitHubPullRequest>();
//     }
// }

public class GitHubPullRequest
{
    public int Number { get; set; }
    public string Title { get; set; }
    public string PullRequestUrl { get; set; }
    public GitHubPullRequest()
    {
    }
    public GitHubPullRequest(int number, string title)
    {
        this.Number = number;
        this.Title = title;
    }
    public GitHubPullRequest(int number, string title, string gitHubUserName, string gitHubRepoName)
    {
        this.Number = number;
        this.Title = title;
        this.PullRequestUrl = number > 0 ? $"https://github.com/{gitHubUserName}/{gitHubRepoName}/pull/{number}" : string.Empty;
    }
}
