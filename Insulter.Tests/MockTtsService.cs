using Insulter.Services;

namespace Insulter.Tests
{
    public class MockTtsService : ITextToSpeechService
    {
       public async Task<List<VoiceLocale>> GetVoiceLocalesAsync()
        {

            return await Task.FromResult<List<VoiceLocale>>(
                [
                    new VoiceLocale("de-DE", "German (Germany)", "de", "DE"),
                    new VoiceLocale("en-GB", "English (UK)", "en", "GB"),
                    new VoiceLocale("en-US", "English (US)", "en", "US"),
                    new VoiceLocale("fr-FR", "French (France)", "fr", "FR"),
                    new VoiceLocale("es-ES", "Spanish (Spain)", "es", "ES"),
                ]);

        } //GetVoiceLocalesAsync
    }


}
