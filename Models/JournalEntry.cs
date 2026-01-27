using SQLite;
using System;
using System.Collections.Generic;

namespace MOODJOURNAL.Models
{
    public class JournalEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public DateTime Date { get; set; }

        [Indexed]
        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty; // HTML Content

        public string ContentRaw { get; set; } = string.Empty; // Plain text for search

        public string PrimaryMood { get; set; } = string.Empty;

        public string SecondaryMoodsJson { get; set; } = "[]"; // Serialized JSON string

        public string TagsJson { get; set; } = "[]"; // Serialized JSON string

        public string Category { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Ignore]
        public List<string> SecondaryMoods { get; set; } = new();

        [Ignore]
        public List<string> Tags { get; set; } = new();
    }
}
