﻿using Newtonsoft.Json;

namespace AccreditTechTest.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "repos_url")]
        public string ReposUrl { get; set; }

        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }
    }
}