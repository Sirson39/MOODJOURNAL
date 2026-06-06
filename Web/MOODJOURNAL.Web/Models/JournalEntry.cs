using System.Collections.Generic;

namespace MOODJOURNAL.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string ContentRaw { get; set; } = string.Empty;

        public string PrimaryMood { get; set; } = string.Empty;

        public string SecondaryMoodsJson { get; set; } = "[]";

        public string TagsJson { get; set; } = "[]";

        public string Category { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<string> SecondaryMoods { get; set; } = new();

        public List<string> Tags { get; set; } = new();
    }
}
