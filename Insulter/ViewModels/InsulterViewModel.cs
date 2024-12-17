using Insulter.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Insulter.ViewModels;

public partial class InsulterViewModel : TextToSpeechViewModel 
{

    private const string WELCOME_MESSAGE = "Salutations! Prithee selectest thou the Shakespearean insult thou wouldst hear me utter.";

    /// <summary>
    /// counter for number of insults spoken
    /// </summary>
    private int _insultsSpoken = 0;
    public int InsultsSpoken
    {
        get => _insultsSpoken;
        private set => SetProperty(ref _insultsSpoken, value);

	} //InsultsSpoken


	/// <summary>
	/// insults list
	/// </summary>
	private ObservableCollection<string> _insultsList = [];
    public ObservableCollection<string> InsultsList
    {
        get => _insultsList; 
        private set => SetProperty(ref _insultsList, value);

	} //InsultsList


	/// <summary>
	/// Creates and initializes new InsulterViewModel object
	/// </summary>
	public InsulterViewModel()
    {

		InsultsList = InsultBuilder.GetInsults();
        Initialized &= InsultsList.Count > 0;

		SpeakingComplete += OnInsultSpoken;

		//timer to delay speaking welcome message at index 0 of insults list until 1 second after app startup
		Application.Current?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
		{
			if (Initialized)
			{
				InsultsList.Insert(0, WELCOME_MESSAGE);
				SpeakNowAsync(InsultsList[0]);
			}

			//terminate timer after speaking of intro phrase has started
			return !Initialized;
		});

	} //InsulterViewModel

    private void OnInsultSpoken(string spokenInsult)
    {
		//count of insults spoken
		InsultsSpoken++;

		try
		{
			//remove insult from list after it has been spoken
			InsultsList.Remove(spokenInsult);
		}
		catch (Exception ex) 
		{
			Debug.WriteLine(ex.Message);
		}

		//load insults if list is empty
		if (InsultsList.Count == 0)
		{
			InsultsList = InsultBuilder.GetInsults();
		}

	} //OnInsultSpoken


} //InsulterViewModel