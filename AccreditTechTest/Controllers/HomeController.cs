using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccreditTechTest.Models;
using AccreditTechTest.Services;

namespace AccreditTechTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGitHubService _gitHubService;

        public HomeController(IGitHubService gitHubService) { _gitHubService = gitHubService; }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(IndexViewModel model)
        {
            if (!_gitHubService.IsValidGitHubUsername(model.SearchViewModel.Username))
            {
                ModelState.AddModelError("SearchViewModel.Username", "Invalid GitHub username.");
                return View();
            }

            var user = await _gitHubService.GetUser(model.SearchViewModel.Username);
            if (user is null)
            {
                ModelState.AddModelError("SearchViewModel.Username", "User does not exist.");
                return View();
            }

            var repos = await _gitHubService.GetRepos(user.ReposUrl);
            var top5Repos = repos
                    .OrderByDescending(r => r.StargazersCount)
                    .Take(5)
                    .ToList();

            var viewModel = new IndexViewModel
            {
                UserReposViewModel = new UserReposViewModel 
                {
                    User = user,
                    Repos = top5Repos
                }
            };

            return View(viewModel);
        }
    }
}