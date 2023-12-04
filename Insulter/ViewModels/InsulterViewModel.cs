using Insulter.Models;

namespace Insulter.ViewModels;

public class InsulterViewModel : TextToSpeechViewModel 
{

    private const string WELCOME_MESSAGE = "Salutations! Prithee selectest thou the Shakespearean insult thou wouldst hear me utter.";

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
        _insultsList = new Insults().InsultsList;
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
				_insultsList = new Insults().InsultsList;
			}
        }

    } //DeleteInsultAt



} //InsulterViewModel