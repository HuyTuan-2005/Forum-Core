namespace Forum.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Gender { get; set; }

        public virtual User User{ get; set; }
    }
}
