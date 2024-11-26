using System.Diagnostics;
using System.Windows.Input;

namespace Insulter.ViewModels;

public partial class TextToSpeechViewModel : ViewModelBase
{

    private const string APP_SETTINGS_LOCALE_INDEX_KEY = nameof(SelectedLocale);
    private const string APP_SETTINGS_VOLUME_KEY = nameof(Volume);
    private const string APP_SETTINGS_PITCH_KEY = nameof(Pitch);


    /// <summary>
    /// Delegate to be called when current utterance has completed
    /// </summary>
    public delegate void SpeakingCompleteHandler();
    public event SpeakingCompleteHandler? SpeakingComplete;


    /// <summary>
    /// Command to initiate speaking of text contained in TextToSpeak property
    /// </summary>
    public ICommand? SpeakNowAsyncCommand { private set; get; } = null;


    /// <summary>
    /// List of locale objects representing TTS voices currently installed on device
    /// </summary>
    private IList<Locale> _localesList = [];
    public IList<Locale> LocalesList
    {
        get { return _localesList; }
        private set { SetProperty(ref _localesList, value); }

    } //LocalesList

        
    /// <summary>
    /// Locale object currently selected from locales list to be used when speaking
    /// </summary>
    private Locale? _selectedLocale = null;
    public Locale? SelectedLocale
    {
        get { return _selectedLocale; }
        set
        {
            if (value is not null  && value != _selectedLocale)
            {
				_selectedLocale = value;
                OnPropertyChanged(nameof(SelectedLocale));
            }
        }

    } //SelectedLocale
    
    
    /// <summary>
    /// true while speaking in progress, false otherwise
    /// </summary>
    private bool _speakingNow = false;
    public bool SpeakingNow
    {
        get { return _speakingNow; }
        private set { SetProperty(ref _speakingNow, value); }

    } //SpeakingNow


    /// <summary>
    /// Speaker volume for voice
    /// </summary>
    const float VOLUME_MIN = 0.0f;
    const float VOLUME_MAX = 1.0f;
    private float _volume = 0.5f;
	public float Volume
    {
        get { return _volume; }
        set
        {
            _volume = Math.Clamp(value, VOLUME_MIN, VOLUME_MAX);
            OnPropertyChanged(nameof(Volume));
        }

    } //Volume


    /// <summary>
    /// Pitch for voice 
    /// </summary>
    const float PITCH_MIN = 0.01f;
    const float PITCH_MAX = 2.0f;
    private float _pitch = 1.0f;
    public float Pitch
    {
        get { return _pitch; }
        set
        {
            _pitch = Math.Clamp(value, PITCH_MIN, PITCH_MAX);
			OnPropertyChanged(nameof(Pitch));
		}

    } //Pitch


    /// <summary>
    /// Text of phrase to be spoken
    /// </summary>
    private string _textToSpeak = "";
    public string TextToSpeak
    {
        get { return _textToSpeak; }
        set { SetProperty(ref _textToSpeak, value); }
    
    } //TextToSpeak


    /// <summary>
    /// true if view model initialization has successfully completed, false otherwise
    /// </summary>
    internal bool _initialized = false;
    public bool Initialized
    {
        get { return _initialized; }
        set { SetProperty(ref _initialized, value); }
    }


	/// <summary>
	/// view model constructor, starts async initialization of view model
	/// </summary>
	public TextToSpeechViewModel()
    {

        InitializeViewModelAsync();

    } //TextToSpeechViewModel


    /// <summary>
    /// prepares locales list, restores view model persisted state, updates command enabled status
    /// </summary>
    /// <returns></returns>
    private async void InitializeViewModelAsync()
    {
		Initialized = false;

		Debug.WriteLine("*** InitializeViewModelAsync: start");
		Stopwatch sw = new();
		sw.Start();

        //build and sort voice locales list
        List<Locale> localesList = [];
        foreach (Locale locale in await TextToSpeech.GetLocalesAsync())
        {
            //Debug.WriteLine(locale.Name);
            localesList.Add(locale);
        }
		Debug.WriteLine($"InitializeViewModelAsync: found {localesList.Count} locales");
		localesList.Sort(new Comparison<Locale>((x, y) => String.Compare(x.Name, y.Name)));
        LocalesList = localesList;
        SelectedLocale = LocalesList[0];

		//view model state can be restored after locales list successfully generated
		RestoreState();

		SpeakNowAsyncCommand = new Command(SpeakNowAsync, canExecute: () => { return !SpeakingNow; });

		((Command)SpeakNowAsyncCommand).ChangeCanExecute();

		Initialized = true;
		Debug.WriteLine("*** InitializeViewModelAsync: completed in {0:N} ms", sw.Elapsed.Milliseconds);

	} //InitializeViewModelAsync


	/// <summary>
    /// saves view model persisted property value in application preferences key/value store
    /// </summary>
    public void SaveState()
	{

        if (LocalesList is not null && SelectedLocale is not null)
        {
            Preferences.Set(APP_SETTINGS_LOCALE_INDEX_KEY, LocalesList.IndexOf(SelectedLocale));
        }

		Preferences.Set(APP_SETTINGS_VOLUME_KEY, Volume);
		Preferences.Set(APP_SETTINGS_PITCH_KEY, Pitch);

	} //SaveState


	/// <summary>
	/// restores view model persisted data from application preferences key/value store,
	/// sets property values to defaults if no persisted property values found
    /// method is private because initialization of LocalesList has to asynchronously complete before 
    /// view model state can be restored
	/// </summary>
	private void RestoreState()
	{

        if (LocalesList is not null)
        {
            int localeIndex = Preferences.Get(APP_SETTINGS_LOCALE_INDEX_KEY, 0);
            if (localeIndex > LocalesList.Count - 1) localeIndex = 0;
            SelectedLocale = LocalesList[localeIndex];
        }

        Volume = Preferences.Get(APP_SETTINGS_VOLUME_KEY, (float)(VOLUME_MAX/2 + VOLUME_MIN));
		Pitch = Preferences.Get(APP_SETTINGS_PITCH_KEY, (float)(PITCH_MAX/2 + PITCH_MIN));

	} //RestoreState


	/// <summary>
	/// disables SpeakNowAsync view model command and calls MAUI API to asynchronously speak text specified 
	/// by TextToSpeaker property using locale (voice), pitch and volume values from respective properties, 
	/// </summary>
	public async void SpeakNowAsync()
    {

        if (!Initialized || LocalesList == null || SpeakNowAsyncCommand == null || LocalesList.Count == 0) 
        { 
            return; 
        }

        SelectedLocale ??= LocalesList[0];
        var speechOptions = new SpeechOptions()
        {
            Volume = Volume,
            Pitch = Pitch,
            //Locale = _locales.ElementAt<Locale>(_random.Next(0, _locales.Count())) //.FirstOrDefault();
            Locale = SelectedLocale
        };

        Debug.WriteLine("SpeakNowAsync(): Language={0}\tName={1}\tText={2}\tVolume={3:N1}\tPitch={4:N1}",
            speechOptions.Locale.Language,
            speechOptions.Locale.Name,
            _textToSpeak,
            speechOptions.Volume,
            speechOptions.Pitch);

        SpeakingNow = true;
        ((Command)SpeakNowAsyncCommand).ChangeCanExecute();
        await TextToSpeech.SpeakAsync(_textToSpeak, speechOptions).ContinueWith((t) =>
        {
            SpeakingNow = false;
            ((Command)SpeakNowAsyncCommand).ChangeCanExecute();
            SpeakingComplete?.Invoke();               

        }, TaskScheduler.FromCurrentSynchronizationContext());

    } //SpeakNowAsync

} //TextToSpeechViewModel