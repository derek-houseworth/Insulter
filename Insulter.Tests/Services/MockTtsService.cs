using Insulter.Services;

namespace Insulter.Tests.Services
{
    public class MockTtsService : ITextToSpeechService
    {
       public async Task<List<VoiceLocale>> GetVoiceLocalesAsync()
        {

            return await Task.FromResult<List<VoiceLocale>>(
            [
                new VoiceLocale("en-US", "", "Microsoft David", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Speech_OneCore\\Voices\\Tokens\\MSTTS_V110_enUS_DavidM"),
                new VoiceLocale("en-US", "", "Microsoft Zira", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Speech_OneCore\\Voices\\Tokens\\MSTTS_V110_enUS_ZiraM"),
                new VoiceLocale("en-US", "", "Microsoft Mark", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Speech_OneCore\\Voices\\Tokens\\MSTTS_V110_enUS_MarkM")
            ]);

        } //GetVoiceLocalesAsync
    }


}
