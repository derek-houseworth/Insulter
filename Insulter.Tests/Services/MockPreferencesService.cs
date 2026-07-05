using Insulter.Services;

namespace Insulter.Tests.Services;

public class MockPreferencesService : IPreferencesService
{
    Dictionary<string, string> _preferences;

    public MockPreferencesService()
    {
        _preferences = [];
    }
    public bool ContainsKey(string key)
    {
        return _preferences.ContainsKey(key);
    }

    public string Get(string key, string defaultValue)
    {
        return ContainsKey(key) ? _preferences[key] : defaultValue;
    }

    public int Get(string key, int defaultValue)
    {
        return ContainsKey(key) ? (int)Convert.ToInt16(_preferences[key]) : defaultValue;
    }

    public float Get(string key, float defaultValue)
    {
        return ContainsKey(key) ? (float)Convert.ToDecimal(_preferences[key]) : defaultValue;
    }

    public void Set(string key, string value)
    {
        if (ContainsKey(key))
        {
            _preferences[key] = value;
        }
        else
        {
            _preferences.Add(key, value);
        }
    }

    public void Set(string key, int value)
    {
        if (ContainsKey(key))
        {
            _preferences[key] = value.ToString();
        }
        else
        {
            _preferences.Add(key, value.ToString());
        }
    }

    public void Set(string key, float value)
    {
        if (ContainsKey(key))
        {
            _preferences[key] = value.ToString();
        }
        else
        {
            _preferences.Add(key, value.ToString());
        }
    }

}