using System;
using System.Threading.Tasks;

namespace HelloThere.Core.Pushshift
{
    /// <summary>
    /// Interacts with the Pushshift Reddit API
    /// See https://github.com/pushshift/api
    /// </summary>
    public interface IPushshiftClient
    {
        Task<SearchSubmissionsResult> SearchSubmissionsAsync(string subredditName, DateTime? after);
    }
}
