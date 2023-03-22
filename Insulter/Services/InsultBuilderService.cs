using System.Reflection;
using System.Text;

namespace Insulter.Services
{
    internal class InsultBuilderService
    {

        private const string INSULT_PREFIX = "Thou art a";
        private string _adjectivesFile, _adverbsFile, _nounsFile;

        public InsultBuilderService(string adjectivesFile, string adverbsFile, string nounsFile) 
        {
            _adjectivesFile = adjectivesFile;
            _adverbsFile = adverbsFile;
            _nounsFile = nounsFile;
        }

        private static bool StartsWithVowel(string word)
        {
            char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
            return vowels.Contains(word.ToLower()[0]);
        }

        public List<string>GetInsultList()
        {
            Random random = new();


            List<string> adjectives = ReadWordListFromResource(_adjectivesFile);
            List<string> adverbs = ReadWordListFromResource(_adverbsFile);
            List<string> nouns = ReadWordListFromResource(_nounsFile);
            var insultsList = new List<string>();

            while (adjectives.Count > 0)
            {

                StringBuilder insult = new(INSULT_PREFIX);

                //choose word from adjectives list
                int wordIndex = random.Next(0, adjectives.Count - 1);
                insult.Append(StartsWithVowel(adjectives[wordIndex]) ? "n " : " ");
                insult.Append($"{adjectives[wordIndex]}, ");
                adjectives.RemoveAt(wordIndex);

                //choose word from adverbs list
                wordIndex = random.Next(0, adverbs.Count - 1);
                insult.Append($"{adverbs[wordIndex]} ");
                adverbs.RemoveAt(wordIndex);

                //choose word from nouns list
                wordIndex = random.Next(0, nouns.Count - 1);
                insult.Append($"{nouns[wordIndex]}!");
                nouns.RemoveAt(wordIndex);

                insultsList.Add(insult.ToString());
            }


            //Debug.WriteLine("loaded {0} insults", InsultsList.Count);

            return insultsList;

        } //LoadInsults

        private static List<string> ReadWordListFromResource(string resourceID)
        {
            List<string> insultWordsList = new();
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceID);
            using StreamReader reader = new(stream);

            string line;
            while (null != (line = reader.ReadLine()))
            {
                insultWordsList.Add(line);
            }

            return insultWordsList;

        } //ReadWordListFromResource

    }


}
