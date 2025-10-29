using Microsoft.AspNetCore.Identity;

namespace Forum.Models
{
    public class User : IdentityUser<int>
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivity {  get; set; }

        public ICollection<Question> Questions { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public Profile Profile { get; set; }

    }
}
