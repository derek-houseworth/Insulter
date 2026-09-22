using Insulter.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Insulter.ViewModels;

public partial class TextToSpeechViewModel : ViewModelBase
{

    private const string APP_SETTINGS_VOICE_KEY = nameof(SelectedVoice);
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

	private List<VoiceLocale> _voiceLocales = [];

    /// <summary>
    /// List of strings representing friendly name of all TTS voices currently installed on device
    /// </summary>
    public ObservableCollection<string> Voices
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
    /// String representing currently selected voice from TTS voices list
    /// </summary>
    public string SelectedVoice
    {
        get;
        set
        {
            if (field != value && Voices.Contains(value))
            {
                SetProperty(ref field, value);
                if (AutoSave && Initialized)
                {
                    SaveState();
                }
            }
        }

    } = string.Empty;


	/// <summary>
	/// true if speaking is possible, i.e. initialization successfully and not currently speaking
	/// </summary>
    public bool CanSpeak
    {
        get;
        private set
        {
            if (field != value)
            {
                SetProperty(ref field, value);
            }
        }
    }  = false;


	/// <summary>
	/// Speaker volume for voice
	/// </summary>
	const float VOLUME_MIN = 0.0f;
    const float VOLUME_MAX = 1.0f;
    public float Volume
    {
        get;
        set
        {
            if (field != value)
            {
                SetProperty(ref field, Math.Clamp(value, VOLUME_MIN, VOLUME_MAX));
                if (AutoSave && Initialized)
                {
                    SaveState();
                }
            }
        }
    } = (VOLUME_MAX+VOLUME_MIN) / 2;


    /// <summary>
    /// Pitch for voice 
    /// </summary>
    const float PITCH_MIN = 0.0f;
    const float PITCH_MAX = 2.0f;
    public float Pitch
    {
        get;
        set
        {
            if (field != value)
            {
                SetProperty(ref field, Math.Clamp(value, PITCH_MIN, PITCH_MAX));
                if (AutoSave && Initialized)
                {
                    SaveState();
                }
            }
        }
    } = (PITCH_MAX+PITCH_MIN)/2;


    /// <summary>
    /// true if view model initialization has successfully completed, false otherwise
    /// </summary>
    public bool Initialized
    {
        get;
        internal set 
        {
            if (field != value)
            { 
                 SetProperty(ref field, value);
            }
        }

    } = false;


    /// <summary>
    /// causes ViewModel to automatically persist any changes to persistable properties when changed
    /// </summary>
    public bool AutoSave
    {
        get;
        set
        {
            if (field != value)
            {
                SetProperty(ref field, value);
            }
        }
    } = true;
    

    private readonly ITextToSpeechService _ttsService;
    private readonly IPreferencesService _prefsService;

    /// <summary>
    /// view model constructor, starts async initialization of view model
    /// </summary>
    public TextToSpeechViewModel(ITextToSpeechService ttsService, IPreferencesService prefsService)
    {
        _ttsService = ttsService ?? throw new ArgumentNullException(nameof(ttsService));
        _prefsService = prefsService ?? throw new ArgumentNullException(nameof(prefsService));

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
        _voiceLocales = await _ttsService.GetVoiceLocalesAsync();
		Debug.WriteLine($"InitializeViewModelAsync: found {_voiceLocales.Count} locales");
        _voiceLocales.Sort(new Comparison<VoiceLocale>((x, y) => String.Compare(x.Name, y.Name)));
        bool isAndroid = DeviceInfo.Current.Platform == DevicePlatform.Android;
		foreach (VoiceLocale voiceLocale in _voiceLocales)
        {
            //locale name string value on Android already contains language and country
            string item = isAndroid ? $"{voiceLocale.Name} - {voiceLocale.Id}" : 
                $"{voiceLocale.Name} ({voiceLocale.Language}{voiceLocale.Country})";            
            Voices.Add(item);
		}

        //restore values of persisted view model properties if values exist and are valid
        RestoreState();

		((Command)SpeakNow).ChangeCanExecute();

		Initialized = true;
        CanSpeak = true;
		Debug.WriteLine("*** InitializeViewModelAsync: completed");


	} //InitializeViewModelAsync


    /// <summary>
    /// restores persisted view model state from previous app sessions, if any exists and is valid
    /// </summary>
    public void RestoreState()
    {

        //SelectedVoice property
        string savedVoice = string.Empty;
        if (_prefsService.ContainsKey(APP_SETTINGS_VOICE_KEY))
        {
            savedVoice = _prefsService.Get(APP_SETTINGS_VOICE_KEY, string.Empty) ?? string.Empty;
        }
        SelectedVoice = (!string.IsNullOrEmpty(savedVoice) && Voices.Contains(savedVoice)) ? savedVoice : Voices[0];

        //Volume property
        if (_prefsService.ContainsKey(APP_SETTINGS_VOLUME_KEY))
        {
            Volume = (float)Math.Round(Math.Clamp(_prefsService.Get(APP_SETTINGS_VOLUME_KEY, (float)(VOLUME_MAX / 2 + VOLUME_MIN)), VOLUME_MIN, VOLUME_MAX),2);
        }

        //Pitch property
        if (_prefsService.ContainsKey(APP_SETTINGS_PITCH_KEY))
        {
            Pitch = (float)Math.Round(Math.Clamp(_prefsService.Get(APP_SETTINGS_PITCH_KEY, (float)(PITCH_MAX / 2 + PITCH_MIN)), PITCH_MIN, PITCH_MAX),2);
        }

    } //RestoreState


    /// <summary>
    /// saves view model persisted property values in application preferences key/value store
    /// </summary>
    public void SaveState()
	{

        _prefsService.Set(APP_SETTINGS_VOICE_KEY, SelectedVoice);
        _prefsService.Set(APP_SETTINGS_VOLUME_KEY, Volume);
        _prefsService.Set(APP_SETTINGS_PITCH_KEY, Pitch);

	} //SaveState


	/// <summary>
	/// disables SpeakNowAsync view model command and calls MAUI API to asynchronously speak text specified 
	/// by TextToSpeaker property using locale (voice), pitch and volume values from respective properties, 
	/// </summary>
	protected async void SpeakNowAsync(string textToSpeak)
    {
        textToSpeak ??= string.Empty;

        //validate view model state and input argument value before proceeding with speaking
        if (!Initialized || !CanSpeak || string.IsNullOrEmpty(textToSpeak)) 
        { 
            return; 
        }

        //CanSpeak flag value set to false indicates view model currently speaking 
        CanSpeak = false;


        //simulate speaking for purpose of unit testing
        if (DeviceInfo.Current.Platform == DevicePlatform.Unknown)
        {
            ((Command)SpeakNow).ChangeCanExecute();
            Task.Delay(25).Wait();
            CanSpeak = true;
            ((Command)SpeakNow).ChangeCanExecute();
            SpeakingComplete?.Invoke(textToSpeak);
            return;
        }

        foreach (var locale in await TextToSpeech.GetLocalesAsync())
        {
            if(_voiceLocales[Voices.IndexOf(SelectedVoice)].Id == locale.Id)
            {
                //speak with 1st voice in list if not previously chosen
                SelectedVoice ??= Voices[0];
                var speechOptions = new SpeechOptions()
                {
                    Volume = Volume,
                    Pitch = Pitch,
                    Locale = locale
                };

                Debug.WriteLine("SpeakNowAsync(): Language: {0}\tName: {1}\tVolume: {2:N1}\tPitch: {3:N1}\t\tText: {4}",
                    speechOptions.Locale.Language,
                    speechOptions.Locale.Name,
                    speechOptions.Volume,
                    speechOptions.Pitch,
                    textToSpeak);

                ((Command)SpeakNow).ChangeCanExecute();
                await TextToSpeech.SpeakAsync(textToSpeak.Trim(), speechOptions).ContinueWith((t) =>
                {
                    CanSpeak = true;
                    ((Command)SpeakNow).ChangeCanExecute();
                    SpeakingComplete?.Invoke(textToSpeak);

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }


    } //SpeakNowAsync

} //TextToSpeechViewModel