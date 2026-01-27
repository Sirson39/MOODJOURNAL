using MOODJOURNAL.Models;

namespace MOODJOURNAL.Services
{
    public class SecurityService
    {
        private readonly DatabaseService _db;

        // Tells the app if the lock screen should be visible
        public bool IsLocked { get; private set; } = true;

        // Stores the details of the person currently logged in
        public User? CurrentUser { get; private set; }

        public SecurityService(DatabaseService db)
        {
            _db = db;
        }

        public async Task<bool> Login(string username, string pin)
        {
            var user = await _db.GetUserAsync(username);
            // Check if user exists and PIN matches exactly
            if (user != null && user.Password == pin)
            {
                CurrentUser = user;
                IsLocked = false;
                return true;
            }
            return false;
        }

        public async Task<bool> Register(string username, string pin)
        {
            var existing = await _db.GetUserAsync(username);
            if (existing != null) return false;

            var newUser = new User
            {
                Username = username,
                Password = pin
            };

            // Save to DB and log them in automatically
            var result = await _db.CreateUserAsync(newUser);
            if (result > 0)
            {
                CurrentUser = await _db.GetUserAsync(username);
                IsLocked = false;
                return true;
            }
            return false;
        }

        public void Lock()
        {
            IsLocked = true;
            CurrentUser = null;
        }
    }
}
