using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace Insulter.Services;

public class InsultBuilder
{

	private const string ADJECTIVES_FILE_NAME = "insultAdjectives.txt";
	private const string ADVERBS_FILE_NAME = "insultAdverbs.txt";
	private const string NOUNS_FILE_NAME = "insultNouns.txt";

	private const string DATA_FILE_PATH = "Insulter.Data.";
	private const string INSULT_PREFIX = "Thou art a";


	/// <summary>
	/// determines if word begins with a vowel
	/// </summary>
	/// <param name="word">string word to check</param>
	/// <returns>returns true if first character in word is a vowel, false otherwise</returns>
	private static bool StartsWithVowel(string word)
    {

		char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
        return vowels.Contains(word.ToLower()[0]);        

	} //StartsWithVowel


	/// <summary>
	/// generatoes list of insults comprised of randomly selected words 
	/// from adjectives, adverbs and nouns lists
	/// </summary>
	/// <returns>List<string> containing insults</string></returns>
	public static ObservableCollection<string>GetInsults(bool singleInsult = false)
    {
        Random random = new();

        List<string> adjectives = ReadWordListFromResource(DATA_FILE_PATH + ADJECTIVES_FILE_NAME),
            adverbs = ReadWordListFromResource(DATA_FILE_PATH + ADVERBS_FILE_NAME),
            nouns = ReadWordListFromResource(DATA_FILE_PATH + NOUNS_FILE_NAME);

		ObservableCollection<string> insults = [];

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

            insults.Add(insult.ToString());

			if (singleInsult) break;
        }

        //System.Diagnostics.Debug.WriteLine($"generated {insultsList.Count} insults");

        return insults;

	} //GetInsults

	public static string GetInsult()
	{
		return GetInsults(true)[0];
	}

	/// <summary>
	/// loads word list from resource file specified by resourceID
	/// </summary>
	/// <param name="resourceId">name of resource file containing words in text format, 1 word per line</param>
	/// <returns>List<string> containing words read from resource file</string></returns>
	private static List<string> ReadWordListFromResource(string resourceId)
    {
		ArgumentNullException.ThrowIfNull(resourceId);

		List<string> insultWordsList = [];
		using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceId);
		if (stream is not null)
		{
            using StreamReader reader = new(stream);
            if (reader is not null)
			{
				string? line = reader.ReadLine();
				while (line is not null) 
				{
					insultWordsList.Add(line);
					line = reader.ReadLine();
				}
			}
		}
		return insultWordsList;

    } //ReadWordListFromResource

} //InsultBuilderService