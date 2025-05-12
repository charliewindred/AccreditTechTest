using System.Collections.Generic;

namespace AccreditTechTest.Models
{
    public class UserReposViewModel
    {
        public User User { get; set; }
        public List<Repo> Repos { get; set; }
    }
}