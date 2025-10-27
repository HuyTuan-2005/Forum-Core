namespace Forum.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public Nullable<int> QuestionId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Body { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }

        public virtual Question Question { get; set; }
        public virtual User User { get; set; }
    }
}
