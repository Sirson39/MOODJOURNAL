using SQLite;
using MOODJOURNAL.Models;
using System.Text.Json;

namespace MOODJOURNAL.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;
        private readonly string _dbPath;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "MoodJournal.db3");
        }

        private async Task Init()
        {
            if (_database is not null)
                return;

            await _semaphore.WaitAsync();
            try
            {
                if (_database is not null)
                    return;

                _database = new SQLiteAsyncConnection(_dbPath);

                // Create tables for Users, Entries, and Categories
                await _database.CreateTableAsync<User>();
                await _database.CreateTableAsync<JournalEntry>();

                try
                {
                    
                    await _database.CreateTableAsync<Category>();

                    // Setup default categories that everyone can use
                    var defaultNames = new[] { "Personal", "Work", "Health", "Travel", "Fitness", "Self-care" };

                    foreach (var name in defaultNames)
                    {
                        var existing = await _database.Table<Category>().Where(c => c.Name == name).FirstOrDefaultAsync();
                        if (existing == null)
                        {
                            await _database.InsertAsync(new Category { Name = name, IsDefault = true, UserId = 0 });
                        }
                        else if (!existing.IsDefault)
                        {
                            existing.IsDefault = true;
                            await _database.UpdateAsync(existing);
                        }
                    }
                }
                catch (Exception catEx)
                {
                
                    System.Diagnostics.Debug.WriteLine($"Category table error: {catEx.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database Init Error: {ex.Message}");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<JournalEntry>> GetEntriesAsync(int userId)
        {
            await Init();
            var entries = await _database!.Table<JournalEntry>()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Date).ToListAsync();
            foreach (var entry in entries)
            {
                entry.SecondaryMoods = JsonSerializer.Deserialize<List<string>>(entry.SecondaryMoodsJson) ?? new();
                entry.Tags = JsonSerializer.Deserialize<List<string>>(entry.TagsJson) ?? new();
            }
            return entries;
        }

        public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date, int userId)
        {
            await Init();
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var entry = await _database!.Table<JournalEntry>()
                .Where(x => x.UserId == userId && x.Date >= startOfDay && x.Date <= endOfDay)
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

        public async Task<List<JournalEntry>> SearchEntriesAsync(string query, int userId)
        {
            await Init();
            var entries = await _database!.Table<JournalEntry>()
                .Where(x => x.UserId == userId && (x.Title.Contains(query) || x.ContentRaw.Contains(query)))
                .ToListAsync();

            foreach (var entry in entries)
            {
                entry.SecondaryMoods = JsonSerializer.Deserialize<List<string>>(entry.SecondaryMoodsJson) ?? new();
                entry.Tags = JsonSerializer.Deserialize<List<string>>(entry.TagsJson) ?? new();
            }
            return entries;
        }

        public async Task<List<JournalEntry>> GetEntriesInDateRangeAsync(DateTime start, DateTime end, int userId)
        {
            await Init();
            var entries = await _database!.Table<JournalEntry>()
                .Where(x => x.UserId == userId && x.Date >= start && x.Date <= end)
                .OrderBy(x => x.Date)
                .ToListAsync();

            foreach (var entry in entries)
            {
                entry.SecondaryMoods = JsonSerializer.Deserialize<List<string>>(entry.SecondaryMoodsJson) ?? new();
                entry.Tags = JsonSerializer.Deserialize<List<string>>(entry.TagsJson) ?? new();
            }
            return entries;
        }

        // Category Management
        public async Task<List<Category>> GetCategoriesAsync(int userId)
        {
            await Init();
            return await _database!.Table<Category>()
                .Where(x => x.UserId == userId || x.UserId == 0)
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<int> SaveCategoryAsync(Category category)
        {
            await Init();
            if (category.Id != 0)
                return await _database!.UpdateAsync(category);
            else
                return await _database!.InsertAsync(category);
        }

        public async Task<int> DeleteCategoryAsync(Category category)
        {
            await Init();
            return await _database!.DeleteAsync(category);
        }

        // User Management
        public async Task<User?> GetUserAsync(string username)
        {
            await Init();
            return await _database!.Table<User>()
                .Where(u => u.Username.ToLower() == username.ToLower())
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateUserAsync(User user)
        {
            await Init();
            return await _database!.InsertAsync(user);
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            await Init();
            return await _database!.UpdateAsync(user);
        }
    }
}
