namespace Insulter.Services
{
    internal class PreferencesService: IPreferencesService
    {

        public bool ContainsKey(string key)
        {
            return Preferences.ContainsKey(key);
        }
        public string Get(string key, string defaultValue)
        { 
            return Preferences.Get(key, defaultValue) ?? defaultValue;
        }
        public float Get(string key, float defaultValue)
        {
            return Preferences.Get(key, defaultValue); 
        }
        public void Set(string key, string value)
        {
            Preferences.Set(key, value);
        }
        public void Set(string key, float value)
        {
            Preferences.Set(key, value);
        }

    }
}
