using System.Security.Cryptography;
using System.Text;
using MOODJOURNAL.Models;

namespace MOODJOURNAL.Services
{
    public class SecurityService
    {
        private const string HashPrefix = "pbkdf2";
        private const int Iterations = 100_000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

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
            var normalizedUsername = NormalizeUsername(username);
            var user = await _db.GetUserAsync(normalizedUsername);
            if (user == null)
                return false;

            if (!VerifyPin(pin, user.Password))
                return false;

            if (!IsHashedPin(user.Password))
            {
                user.Password = HashPin(pin);
                await _db.UpdateUserAsync(user);
            }

            CurrentUser = user;
            IsLocked = false;
            return true;
        }

        public async Task<bool> Register(string username, string pin)
        {
            var normalizedUsername = NormalizeUsername(username);
            var existing = await _db.GetUserAsync(normalizedUsername);
            if (existing != null) return false;

            var newUser = new User
            {
                Username = normalizedUsername,
                Password = HashPin(pin)
            };

            // Save to DB and log them in automatically
            var result = await _db.CreateUserAsync(newUser);
            if (result > 0)
            {
                CurrentUser = await _db.GetUserAsync(normalizedUsername);
                IsLocked = false;
                return true;
            }
            return false;
        }

        public async Task<bool> ChangePinAsync(string currentPin, string newPin)
        {
            if (CurrentUser == null)
                return false;

            if (!VerifyPin(currentPin, CurrentUser.Password))
                return false;

            CurrentUser.Password = HashPin(newPin);
            await _db.UpdateUserAsync(CurrentUser);
            return true;
        }

        public void Lock()
        {
            IsLocked = true;
            CurrentUser = null;
        }

        private static bool IsHashedPin(string value)
        {
            return value.StartsWith($"{HashPrefix}$", StringComparison.Ordinal);
        }

        private static string HashPin(string pin)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(pin),
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return string.Join('$',
                HashPrefix,
                Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));
        }

        private static bool VerifyPin(string pin, string storedValue)
        {
            if (string.IsNullOrWhiteSpace(storedValue))
                return false;

            if (!IsHashedPin(storedValue))
                return storedValue == pin;

            var parts = storedValue.Split('$', 4);
            if (parts.Length != 4)
                return false;

            if (!int.TryParse(parts[1], out var iterations))
                return false;

            try
            {
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expectedHash = Convert.FromBase64String(parts[3]);
                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(pin),
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length);

                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch
            {
                return false;
            }
        }

        private static string NormalizeUsername(string username)
        {
            return username.Trim().ToLowerInvariant();
        }
    }
}
