namespace MOODJournal.Services
{
    public class SecurityService
    {
        private const string PinKey = "journal_pin";
        private const string IsLockedKey = "is_locked";

        public bool IsLocked { get; private set; } = true;

        public SecurityService()
        {
            // Default PIN is 1234 as per requirements/screenshots
            if (string.IsNullOrEmpty(Preferences.Default.Get(PinKey, "")))
            {
                Preferences.Default.Set(PinKey, "1234");
            }
        }

        public bool Unlock(string pin)
        {
            var savedPin = Preferences.Default.Get(PinKey, "1234");
            if (pin == savedPin)
            {
                IsLocked = false;
                return true;
            }
            return false;
        }

        public void Lock()
        {
            IsLocked = true;
        }

        public string GetSavedPin() => Preferences.Default.Get(PinKey, "1234");

        public void SetPin(string newPin)
        {
            Preferences.Default.Set(PinKey, newPin);
        }
    }
}
