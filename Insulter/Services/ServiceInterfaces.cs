namespace Insulter.Services;

public interface ITextToSpeechService
{
    Task<List<VoiceLocale>> GetVoiceLocalesAsync();
}

public interface IPreferencesService
{

    bool ContainsKey(string key);
    string Get(string key, string defaultValue);
    float Get(string key, float defaultValue);

    void Set(string key, string value);
    void Set(string key, float value);

}
