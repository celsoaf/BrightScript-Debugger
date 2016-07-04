using System.Diagnostics;
using System.Linq;
using LibGit2Sharp;

namespace RokuTelnet.Services.Git
{
    public class GitService : IGitService
    {
        public string Describe(string path)
        {
            using (var repo = new Repository(path))
            {
                var commit = repo.Commits.First();
                var version = repo.Describe(commit, new DescribeOptions() {Strategy = DescribeStrategy.Tags});

                return version;
            }

            return null;
        }
    }
}