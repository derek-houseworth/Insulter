namespace Insulter.Services;

public interface ITextToSpeechService
{
    Task<List<VoiceLocale>> GetVoiceLocalesAsync();
}

public class TextToSpeechService : ITextToSpeechService
{
    public async Task<List<VoiceLocale>> GetVoiceLocalesAsync() 
    {
        List<VoiceLocale> voices = [];
        foreach(var locale in await TextToSpeech.GetLocalesAsync())
        {
            voices.Add(new VoiceLocale(locale.Language, locale.Country, locale.Name, locale.Id));
        }
        return voices;

    }

} //TextToSpeechService