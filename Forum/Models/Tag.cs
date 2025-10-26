

namespace Forum.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public string TagName { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
