using AccreditTechTest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccreditTechTest.Services
{
    public interface IGitHubService
    {
        Task<User> GetUser(string username);
        Task<List<Repo>> GetRepos(string reposUrl);
        bool IsValidGitHubUsername(string username);

    }
}
