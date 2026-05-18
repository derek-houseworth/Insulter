namespace Insulter.Services
{
    public class VoiceLocale(string language, string country, string name, string id)
    {
        public string Language { get; private set; } = language;
        public string Country { get; private set; } = country;
        public string Name { get; private set; } = name;
        public string Id { get; private set; } = id;
    }
}
