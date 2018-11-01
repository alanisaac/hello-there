using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelloThere.Core.Pushshift
{
    public interface ISubmissionRepository
    {
        Task BulkInsertSubmissionsAsync(IEnumerable<Submission> submissions);

        Task<DateTime?> GetMaxCreatedDateAsync();
    }
}
