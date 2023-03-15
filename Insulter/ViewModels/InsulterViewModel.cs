using Insulter.Models;

namespace Insulter.ViewModels;

public class InsulterViewModel : TextToSpeechViewModel 
{

    private string _welcomeMessage = "Salutations! Prithee selectest thou the Shakespearean insult thou wouldst hear me utter!";

    /// <summary>
    /// insults list
    /// </summary>
    private List<string> _insultsList;
    public List<string> InsultsList
    {
        get => _insultsList; 
        private set { SetProperty(ref _insultsList, value); }

    } //insults list


    public InsulterViewModel()
    {
        _insultsList = new Insults().InsultsList;
        InsultsList.Insert(0, _welcomeMessage);
    }

    public void SpeakInsult(int index)
    {
        TextToSpeak = InsultsList[index];
        DeleteInsultAt(index);
        SpeakNowAsync();
    }
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
    }

 



} //InsulterViewModel
