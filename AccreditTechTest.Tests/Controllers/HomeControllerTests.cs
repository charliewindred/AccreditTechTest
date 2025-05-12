using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccreditTechTest.Controllers;
using AccreditTechTest.Models;
using AccreditTechTest.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AccreditTechTest.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<IGitHubService> _mockGitHubService;
        private HomeController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockGitHubService = new Mock<IGitHubService>();
            _controller = new HomeController(_mockGitHubService.Object);
        }

        [TestMethod]
        public void Index()
        {
            // Act
            ViewResult result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Index_UserDoesNotExist_AddsModelErrorAndReturnsView()
        {
            // Arrange
            var username = "tester";
            var model = new IndexViewModel
            {
                SearchViewModel = new SearchViewModel { Username = username }
            };

            _mockGitHubService.Setup(x => x.IsValidGitHubUsername(username)).Returns(true);
            _mockGitHubService.Setup(x => x.GetUser(username)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Index(model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsTrue(!_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState.ContainsKey("SearchViewModel.Username"));
            Assert.AreEqual("User does not exist.",
                _controller.ModelState["SearchViewModel.Username"].Errors.First().ErrorMessage);
        }

        [TestMethod]
        public async Task Index_ValidUserAndRepos_ReturnsViewWithTop5Repos()
        {
            // Arrange
            var username = "tester";
            var user = new User { ReposUrl = "https://api.github.com/users/tester/repos" };
            var repos = Enumerable.Range(1, 10)
                .Select(i => new Repo { Name = $"Repo{i}", StargazersCount = i })
                .ToList();

            var model = new IndexViewModel
            {
                SearchViewModel = new SearchViewModel { Username = username }
            };

            _mockGitHubService.Setup(x => x.IsValidGitHubUsername(username)).Returns(true);
            _mockGitHubService.Setup(x => x.GetUser(username)).ReturnsAsync(user);
            _mockGitHubService.Setup(x => x.GetRepos(user.ReposUrl)).ReturnsAsync(repos);

            // Act
            var result = await _controller.Index(model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var resultModel = viewResult.Model as IndexViewModel;
            Assert.IsNotNull(resultModel);
            Assert.AreEqual(5, resultModel.UserReposViewModel.Repos.Count);
            Assert.AreEqual("Repo10", resultModel.UserReposViewModel.Repos.First().Name);
        }
    }
}

