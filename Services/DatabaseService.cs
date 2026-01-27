using SQLite;
using MOODJOURNAL.Models;
using System.Text.Json;

namespace MOODJOURNAL.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;
        private readonly string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "MoodJournal.db3");
        }

        private async Task Init()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(_dbPath);
            await _database.CreateTableAsync<JournalEntry>();
        }

        public async Task<List<JournalEntry>> GetEntriesAsync()
        {
            await Init();
            var entries = await _database!.Table<JournalEntry>().OrderByDescending(x => x.Date).ToListAsync();
            foreach (var entry in entries)
            {
                entry.SecondaryMoods = JsonSerializer.Deserialize<List<string>>(entry.SecondaryMoodsJson) ?? new();
                entry.Tags = JsonSerializer.Deserialize<List<string>>(entry.TagsJson) ?? new();
            }
            return entries;
        }

        public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
        {
            await Init();
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var entry = await _database!.Table<JournalEntry>()
                .Where(x => x.Date >= startOfDay && x.Date <= endOfDay)
                .FirstOrDefaultAsync();

            if (entry != null)
            {
                entry.SecondaryMoods = JsonSerializer.Deserialize<List<string>>(entry.SecondaryMoodsJson) ?? new();
                entry.Tags = JsonSerializer.Deserialize<List<string>>(entry.TagsJson) ?? new();
            }

            return entry;
        }

        public async Task<int> SaveEntryAsync(JournalEntry entry)
        {
            await Init();
            entry.SecondaryMoodsJson = JsonSerializer.Serialize(entry.SecondaryMoods);
            entry.TagsJson = JsonSerializer.Serialize(entry.Tags);
            entry.UpdatedAt = DateTime.Now;

            if (entry.Id != 0)
            {
                return await _database!.UpdateAsync(entry);
            }
            else
            {
                entry.CreatedAt = DateTime.Now;
                return await _database!.InsertAsync(entry);
            }
        }

        public async Task<int> DeleteEntryAsync(JournalEntry entry)
        {
            await Init();
            return await _database!.DeleteAsync(entry);
        }

        public async Task<List<JournalEntry>> SearchEntriesAsync(string query)
        {
            await Init();
            var entries = await _database!.Table<JournalEntry>()
                .Where(x => x.Title.Contains(query) || x.ContentRaw.Contains(query))
                .ToListAsync();

            foreach (var entry in entries)
            {
                entry.SecondaryMoods = JsonSerializer.Deserialize<List<string>>(entry.SecondaryMoodsJson) ?? new();
                entry.Tags = JsonSerializer.Deserialize<List<string>>(entry.TagsJson) ?? new();
            }
            return entries;
        }

        public async Task<List<JournalEntry>> GetEntriesInDateRangeAsync(DateTime start, DateTime end)
        {
            await Init();
            var entries = await _database!.Table<JournalEntry>()
                .Where(x => x.Date >= start && x.Date <= end)
                .OrderBy(x => x.Date)
                .ToListAsync();

            foreach (var entry in entries)
            {
                entry.SecondaryMoods = JsonSerializer.Deserialize<List<string>>(entry.SecondaryMoodsJson) ?? new();
                entry.Tags = JsonSerializer.Deserialize<List<string>>(entry.TagsJson) ?? new();
            }
            return entries;
        }
    }
}
