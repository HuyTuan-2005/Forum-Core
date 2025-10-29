using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreateAt { get; set; }


        public User User { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Answer> Answer { get; set; }

        public Question()
        {
            Tags = new List<Tag>();
        }
    }
}
