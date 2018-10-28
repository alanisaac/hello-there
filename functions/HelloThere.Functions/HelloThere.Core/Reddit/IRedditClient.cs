using System.Threading.Tasks;

namespace HelloThere.Core.Reddit
{
    public interface IRedditClient
    {
        Task<GetPostsResult> GetTopPostsAsync(string subredditName, int count);
        
        Task<GetPostsResult> GetNewPostsAsync(string subredditName, int count);
    }
}
