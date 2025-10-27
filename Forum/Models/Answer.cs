namespace Forum.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public Nullable<int> QuestionId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Body { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }

        public Question Question { get; set; }
        public User User { get; set; }
    }
}
