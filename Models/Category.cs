using SQLite;

namespace MOODJOURNAL.Models
{
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, Indexed]
        public string Name { get; set; } = "";

        public string Color { get; set; } = "#C46210"; // Default accent color

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
