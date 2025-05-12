using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AccreditTechTest.Models;
using AccreditTechTest.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace AccreditTechTest.Tests.Controllers
{
    [TestClass]
    public class GitHubServiceTests
    {
        private Mock<HttpMessageHandler> _handlerMock;
        private GitHubService _service;

        [TestInitialize]
        public void Setup()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.github.com/")
            };

            _service = new GitHubService(httpClient);
        }

        [TestMethod]
        public void IsValidGitHubUsername_ValidUsername_ReturnsTrue()
        {
            // Act
            var result = _service.IsValidGitHubUsername("valid-user123");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GetUser_ValidUsername_ReturnsUserObject()
        {
            // Arrange
            var mockUser = new User
            {
                Location = "test",
                Name = "tester",
                AvatarUrl = "test",
                ReposUrl = "test",
            };

            var responseContent = JsonConvert.SerializeObject(mockUser);

            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("users/tester")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Act
            var result = await _service.GetUser("tester");

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
