using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        
        [MaxLength(55)]
        public string DisplayName { get; set; }
        public Nullable<DateTime> Birthday { get; set; }
        public string Gender { get; set; }

        public User User{ get; set; }
    }
}
