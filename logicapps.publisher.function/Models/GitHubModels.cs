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
    public long Id { get; set; }
    public string Title { get; set; }
    public GitHubPullRequest()
    {
    }
    public GitHubPullRequest(long Id, string title)
    {
        this.Id = Id;
        this.Title = title;
    }
}
