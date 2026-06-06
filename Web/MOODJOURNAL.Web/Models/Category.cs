namespace MOODJOURNAL.Models
{
    public class Category
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; } = "";

        public string Color { get; set; } = "#C46210";

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
