using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Insulter.ViewModels;

public partial class TextToSpeechViewModel : ViewModelBase
{

    private const string APP_SETTINGS_LOCALE_INDEX_KEY = nameof(SelectedVoice);
    private const string APP_SETTINGS_VOLUME_KEY = nameof(Volume);
    private const string APP_SETTINGS_PITCH_KEY = nameof(Pitch);


    /// <summary>
    /// Delegate to be called when current utterance has completed
    /// </summary>
    public delegate void SpeakingCompleteHandler(string spokenText);
    public event SpeakingCompleteHandler? SpeakingComplete;


    /// <summary>
    /// Command to initiate speaking of text contained in TextToSpeak property
    /// </summary>
    public ICommand SpeakNow { private set; get; }

	private readonly List<Locale> _locales = [];

	/// <summary>
	/// List of strings representing friendly name of all TTS voices currently installed on device
	/// </summary>
	private ObservableCollection<string> _voices = [];
    public ObservableCollection<string> Voices
    {
        get => _voices;
        private set => SetProperty(ref _voices, value);

	} //Voices


	/// <summary>
	/// String representing currently selected voice from TTS voices list
	/// </summary>
	private string _selectedVoice = string.Empty;
    public string SelectedVoice
    {
        get =>_selectedVoice;
        set 
        {
            if (value != _selectedVoice && Voices.Contains(value))
            {
				SetProperty(ref _selectedVoice, value);
			}			
		} 

    } //SelectedLocale

	/// <summary>
	/// true if speaking is possible, i.e. initialization successfully and not currently speaking
	/// </summary>
	private bool _canSpeak = false;
    public bool CanSpeak
    {
        get => _canSpeak;
        private set => SetProperty(ref _canSpeak, value);

	} //CanSpeak


	/// <summary>
	/// Speaker volume for voice
	/// </summary>
	const float VOLUME_MIN = 0.0f;
    const float VOLUME_MAX = 1.0f;
    private float _volume = 0.5f;
	public float Volume
    {
        get => _volume;
        set => SetProperty(ref _volume, Math.Clamp(value, VOLUME_MIN, VOLUME_MAX));

    } //Volume


    /// <summary>
    /// Pitch for voice 
    /// </summary>
    const float PITCH_MIN = 0.01f;
    const float PITCH_MAX = 2.0f;
    private float _pitch = 1.0f;
    public float Pitch
    {
        get => _pitch;
        set => SetProperty(ref _pitch, Math.Clamp(value, PITCH_MIN, PITCH_MAX));       

    } //Pitch


    /// <summary>
    /// true if view model initialization has successfully completed, false otherwise
    /// </summary>
    internal bool _initialized = false;
    public bool Initialized
    {
        get => _initialized;
        set => SetProperty(ref _initialized, value);

	} //Initialized


	/// <summary>
	/// view model constructor, starts async initialization of view model
	/// </summary>
	public TextToSpeechViewModel()
    {
		SpeakNow = new Command<string>(
        	execute: (string textToSpeak) => SpeakNowAsync(textToSpeak),
	        canExecute: (string textToSpeak) => { return CanSpeak; }
	    );

		InitializeViewModelAsync();

    } //TextToSpeechViewModel


    /// <summary>
    /// prepares locales list, restores view model persisted state, updates command enabled status
    /// </summary>
    /// <returns></returns>
    private async void InitializeViewModelAsync()
    {
		Initialized = false;
        CanSpeak = false;

		Debug.WriteLine("*** InitializeViewModelAsync: start");

        //build and sort voice locales list
        foreach (Locale locale in await TextToSpeech.GetLocalesAsync())
        {
            //Debug.WriteLine(locale.Name);
            _locales.Add(locale);
        }
		Debug.WriteLine($"InitializeViewModelAsync: found {_locales.Count} locales");
		_locales.Sort(new Comparison<Locale>((x, y) => String.Compare(x.Name, y.Name)));
        foreach (Locale locale in _locales)
        {
            //locale name string value on Android already contains language and country 
            string item = DeviceInfo.Current.Platform == DevicePlatform.Android ? locale.Name :
                $"{locale.Name} ({locale.Country}{locale.Language})";
			Voices.Add(item);
		}
        SelectedVoice = Voices[0];

		//view model state can be restored after locales list successfully generated
		RestoreState();

		((Command)SpeakNow).ChangeCanExecute();

		Initialized = true;
        CanSpeak = true;
		Debug.WriteLine("*** InitializeViewModelAsync: completed");


	} //InitializeViewModelAsync


	/// <summary>
    /// saves view model persisted property value in application preferences key/value store
    /// </summary>
    public void SaveState()
	{

        if (Voices is not null && SelectedVoice is not null)
        {
            Preferences.Set(APP_SETTINGS_LOCALE_INDEX_KEY, Voices.IndexOf(SelectedVoice));
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

        if (Voices is not null)
        {
            int selectedVoiceIndex = Preferences.Get(APP_SETTINGS_LOCALE_INDEX_KEY, 0);
            if (selectedVoiceIndex < 0 || selectedVoiceIndex > Voices.Count - 1)
            {
				selectedVoiceIndex = 0;
			}            
            SelectedVoice = Voices[selectedVoiceIndex];
        }

        Volume = Math.Clamp(Preferences.Get(APP_SETTINGS_VOLUME_KEY, (float)(VOLUME_MAX / 2 + VOLUME_MIN)), VOLUME_MIN, VOLUME_MAX);

		Pitch = Math.Clamp(Preferences.Get(APP_SETTINGS_PITCH_KEY, (float)(PITCH_MAX / 2 + PITCH_MIN)), PITCH_MIN, PITCH_MAX);

	} //RestoreState


	/// <summary>
	/// disables SpeakNowAsync view model command and calls MAUI API to asynchronously speak text specified 
	/// by TextToSpeaker property using locale (voice), pitch and volume values from respective properties, 
	/// </summary>
	protected async void SpeakNowAsync(string textToSpeak)
    {
        if (!Initialized || !CanSpeak || Voices == null || Voices.Count == 0 || textToSpeak is null) 
        { 
            return; 
        }
        CanSpeak = false;

		SelectedVoice ??= Voices[0];
        var speechOptions = new SpeechOptions()
        {
            Volume = Volume,
            Pitch = Pitch,
            //Locale = _locales.ElementAt<Locale>(_random.Next(0, _locales.Count())) //.FirstOrDefault();
            Locale = _locales[Voices.IndexOf(SelectedVoice)]
        };

        Debug.WriteLine("SpeakNowAsync(): Language={0}\tName={1}\tText={2}\tVolume={3:N1}\tPitch={4:N1}",
            speechOptions.Locale.Language,
            speechOptions.Locale.Name,
            textToSpeak,
            speechOptions.Volume,
            speechOptions.Pitch);

        ((Command)SpeakNow).ChangeCanExecute();
        await TextToSpeech.SpeakAsync(textToSpeak, speechOptions).ContinueWith((t) =>
        {
            CanSpeak = true;
            ((Command)SpeakNow).ChangeCanExecute();
            SpeakingComplete?.Invoke(textToSpeak);               

        }, TaskScheduler.FromCurrentSynchronizationContext());

    } //SpeakNowAsync

} //TextToSpeechViewModel