using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        
        [Required]
        [MaxLength(30)]
        public string TagName { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
