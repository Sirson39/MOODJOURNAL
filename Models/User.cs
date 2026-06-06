using SQLite;

namespace MOODJOURNAL.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Username { get; set; } = "";

        public string Password { get; set; } = ""; // Stores the hashed PIN/password value

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
