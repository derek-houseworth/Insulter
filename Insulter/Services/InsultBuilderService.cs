using System.Collections.ObjectModel;
using System.Text;

namespace Insulter.Services;

public class InsultBuilderService
{

	private const string ADJECTIVES_FILE_NAME = "insultAdjectives.txt";
	private const string ADVERBS_FILE_NAME = "insultAdverbs.txt";
	private const string NOUNS_FILE_NAME = "insultNouns.txt";

	private const string INSULT_PREFIX = "Thou art a";

    private static readonly char[] _vowels = ['a', 'e', 'i', 'o', 'u'];


    /// <summary>
    /// determines if word begins with a vowel
    /// </summary>
    /// <param name="word">string word to check</param>
    /// <returns>returns true if first character in word is a vowel, false otherwise</returns>
    private static bool StartsWithVowel(string word)
    {
		
        return _vowels.Contains(word.ToLower()[0]);        

	} //StartsWithVowel


	/// <summary>
	/// generatoes list of insults comprised of randomly selected words 
	/// from adjectives, adverbs and nouns lists
	/// </summary>
	/// <returns>List<string> containing insults</string></returns>
	public static async Task<ObservableCollection<string>>GetInsults(string alternatePath = "")
    {
        Random random = new();

        string insultsDir = AppContext.BaseDirectory;

        List<string> adjectives = await ReadWordList(ADJECTIVES_FILE_NAME, alternatePath),
            adverbs = await ReadWordList(ADVERBS_FILE_NAME, alternatePath),
            nouns = await ReadWordList(NOUNS_FILE_NAME, alternatePath);

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
			
        }

        //System.Diagnostics.Debug.WriteLine($"generated {insultsList.Count} insults");

        return insults;

	} //GetInsults
	

	/// <summary>
	/// loads word list from resource file in executing assembly specified by resourceId
	/// </summary>
	/// <param name="fileName">name of file containing words in text format, 1 word per line</param>
	/// <returns>List<string> containing words read from resource file</string></returns>
	private static async Task <List<string>> ReadWordList(string fileName, string alternatePath = "")
    {
		List<string> insultWordsList = [];

        using var stream = string.IsNullOrEmpty(alternatePath) ? await FileSystem.OpenAppPackageFileAsync(fileName) :
            File.OpenRead(System.IO.Path.Combine(alternatePath, fileName));
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