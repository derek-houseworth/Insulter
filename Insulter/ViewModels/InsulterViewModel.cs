using Insulter.Models;
using System.Diagnostics;

namespace Insulter.ViewModels;

public class InsulterViewModel : TextToSpeechViewModel 
{

    private const string DATA_FILE_PATH = "Insulter.Data.";
	private const string ADJECTIVES_FILE_NAME = DATA_FILE_PATH  + "insultAdjectives.txt";
    private const string ADVERBS_FILE_NAME = DATA_FILE_PATH + "insultAdverbs.txt";
    private const string NOUNS_FILE_NAME = DATA_FILE_PATH + "insultNouns.txt";

    private const string WELCOME_MESSAGE = "Salutations! Prithee selectest thou the Shakespearean insult thou wouldst hear me utter.";

    private readonly Insults _insults;


	/// <summary>
	/// insults list
	/// </summary>
	private List<string> _insultsList;
    public List<string> InsultsList
    {
        get => _insultsList; 
        private set => SetProperty(ref _insultsList, value);

    } //insults list


    /// <summary>
    /// Creates and initializes new InsulterViewModel object
    /// </summary>
    public InsulterViewModel()
    {
        _insults = new Insults(ADJECTIVES_FILE_NAME, ADVERBS_FILE_NAME, NOUNS_FILE_NAME);

        _insultsList = _insults.InsultsList;
        _initialized &= InsultsList.Count > 0;
        InsultsList.Insert(0, WELCOME_MESSAGE);

    } //InsulterViewModel


    /// <summary>
    /// Speaks insult from insult list at specified positio 
    /// </summary>
    /// <param name="insultIndex">Integer index of insult in InsultsList to speak</param>
    public void SpeakInsult(int insultIndex)
    {

        TextToSpeak = InsultsList[insultIndex];
        DeleteInsultAt(insultIndex);
        SpeakNowAsync();

    } //SpeakInsult


    /// <summary>
    /// Removes insult from list at specified index and re-initializes insults list if empty
    /// </summary>
    /// <param name="index">Integer list index of insult to be removed</param>
    private void DeleteInsultAt(int index)
    {
        if (index >= 0 && index < _insultsList.Count)
        {
            InsultsList.RemoveAt(index);
            if (0 == InsultsList.Count)
            {
                _insultsList = _insults.InsultsList;
            }
        }

    } //DeleteInsultAt



} //InsulterViewModel