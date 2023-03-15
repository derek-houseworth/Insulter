using System.Reflection;

namespace Insulter.Models
{
    public class Insults
    {
        private const string ADJECTIVES_FILE_NAME = "insultAdjectives.txt";
        private const string ADVERBS_FILE_NAME = "insultAdverbs.txt";
        private const string NOUNS_FILE_NAME = "insultNouns.txt";

        private const string DEFAULT_FILE_RESOURCE_ID_PREFIX = "Insulter.InsultData.";

        private readonly string _resourceIdPrefix;
        private readonly Random _random = new();

        public List<string> InsultsList;
        
        public Insults(string resourceIDPrefix = DEFAULT_FILE_RESOURCE_ID_PREFIX) 
        {

            _resourceIdPrefix  = resourceIDPrefix;
            LoadInsults();

        } //Insults

        private static List<string> ReadWordListFromResource(string resourceID)
        {
            var insultWordsList = new List<string>();
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceID);
            using StreamReader reader = new(stream);
            string line;
            while (null != (line = reader.ReadLine()))
            {
                insultWordsList.Add(line);
            }

            return insultWordsList;

        } //ReadWordListFromResource

        private void LoadInsults()
        {

            List<string> adjectives = ReadWordListFromResource(_resourceIdPrefix + ADJECTIVES_FILE_NAME);
            List<string> adverbs = ReadWordListFromResource(_resourceIdPrefix + ADVERBS_FILE_NAME);
            List<string> nouns = ReadWordListFromResource(_resourceIdPrefix + NOUNS_FILE_NAME);
            InsultsList = new List<string>();

            while (adjectives.Count > 0)
            {

                string insult = "Thou art a";
                string vowels = "aeiou";

                //choose word from adjectives list
                int wordIndex = _random.Next(0, adjectives.Count - 1);
                string insultPart = adjectives[wordIndex];
                insult += (vowels.IndexOf(insultPart.Substring(0, 1).ToLower()) >= 0) ? "n " : " ";
                insult += (insultPart + ", ");
                adjectives.RemoveAt(wordIndex);

                //choose word from adverbs list
                wordIndex = _random.Next(0, adverbs.Count - 1);
                insultPart = adverbs[wordIndex];
                insult += (insultPart + " ");
                adverbs.RemoveAt(wordIndex);

                //choose word from nouns list
                wordIndex = _random.Next(0, nouns.Count - 1);
                insultPart = nouns[wordIndex];
                insult += (insultPart + "! ");
                nouns.RemoveAt(wordIndex);

                InsultsList.Add(insult);
            }

            //Debug.WriteLine("loaded {0} insults", InsultsList.Count);

        } //LoadInsults

    } //Insults

} //Insulter.Models
