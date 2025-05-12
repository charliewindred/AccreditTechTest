using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AccreditTechTest.Models;
using Newtonsoft.Json;
using Serilog;

namespace AccreditTechTest.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        public GitHubService(HttpClient httpClient) { _httpClient = httpClient;  }

        public bool IsValidGitHubUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            if (username.Length > 39)
                return false;

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9-]+$"))
                return false;

            if (username.StartsWith("-") || username.EndsWith("-"))
                return false;

            if (username.Contains("--"))
                return false;

            return true;
        }

        public async Task<User> GetUser(string username) 
        {
            return await Get<User>(new Uri($"users/{username}", UriKind.Relative));
        }

        public async Task<List<Repo>> GetRepos(string reposUri) 
        {
            return await Get<List<Repo>>(new Uri(reposUri, UriKind.RelativeOrAbsolute));
        }

        private async Task<T> Get<T>(Uri uri)
        {
            try
            {
                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(json);

                if (result == null)
                {
                    Log.Warning("Deserialization returned null for URI: {Uri}", uri);
                }

                return result;
            }
            catch (HttpRequestException httpEx)
            {
                Log.Error(httpEx, "HTTP request failed.");
                return default;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while deserializing response.");
                return default;
            }
        }
    }
}