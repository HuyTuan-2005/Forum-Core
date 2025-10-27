namespace Forum.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreateAt { get; set; }


        public virtual User User { get; set; }
        public ICollection<Tag> Tags { get; set; }

        public Question()
        {
            Tags = new List<Tag>();
        }
    }
}
