using Insulter.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Insulter.ViewModels;

public partial class InsulterViewModel : TextToSpeechViewModel 
{

    /// <summary>
    /// count of number of insults spoken
    /// </summary>
    public int InsultsSpoken
    {
        get;
        private set
        {
            if (field != value)
            {
                SetProperty(ref field, value);
            }
        }
    } = 0;
    

    /// <summary>
    /// insults list
    /// </summary>
    public ObservableCollection<string> InsultsList
    {
        get;
        private set
        {
            if (field != value)
            {
                SetProperty(ref field, value);
            }
        }
    } = [];
    

    /// <summary>
    /// string containing currently selected insult
    /// </summary>
    public string SelectedInsult
    {
        get;
        set
        {
            if (field != value)
            {
                SetProperty(ref field, value);
            }
        }
    } = string.Empty;
    

    /// <summary>
    /// Creates and initializes new InsulterViewModel object
    /// </summary>
    public InsulterViewModel(ITextToSpeechService ttsService, IPreferencesService prefsService) : base(ttsService, prefsService)
	{
        //register call-back for when insult has been spoken
        SpeakingComplete += OnInsultSpoken;

        //initialize insults list with insults from insult builder service and insert welcome message at index 0
        InsultsList = InsultBuilderService.GetInsults();
        InsultsList.Insert(0, Properties.Resources.WelcomeMessage);
        Initialized &= InsultsList.Count > 1;

        //timer to delay speaking welcome message at index 0 of insults list until 1 second after app startup
        Application.Current?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
		{
			if (Initialized)
			{
				
                SelectedInsult = Properties.Resources.WelcomeMessage;
                //SpeakNowAsync(InsultsList[0]);
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
			InsultsList = InsultBuilderService.GetInsults();
		}

	} //OnInsultSpoken


} //InsulterViewModel