using System.Diagnostics;
using System.Windows.Input;

namespace Insulter.ViewModels;

public class TextToSpeechViewModel : ViewModelBase
{

    /// <summary>
    /// Delegate to be called when current utterance has completed
    /// </summary>
    public delegate void SpeakingCompleteHandler();
    public event SpeakingCompleteHandler SpeakingComplete;

    /// <summary>
    /// Command to utter text from TextToSpeak property
    /// </summary>
    public ICommand SpeakNowAsyncCommand { private set; get; }


    private IList<Locale> _localesList;        
    /// <summary>
    /// List of locale objects representing TTS voices currently installed on device.
    /// </summary>
    public IList<Locale> LocalesList
    {
        get { return _localesList; }
        private set { SetProperty(ref _localesList, value); }

    } //LocalesList

    private Locale _selectedLocale;
    /// <summary>
    /// 
    /// </summary>
    public Locale SelectedLocale
    {
        get { return _selectedLocale; }
        set { SetProperty(ref _selectedLocale, value); }

    } //SelectedLocale


    /// <summary>
    /// 
    /// </summary>
    private bool _speakingNow;
    public bool SpeakingNow
    {
        get { return _speakingNow; }
        private set { SetProperty(ref _speakingNow, value); }

    } //SpeakingNow


    const double VOLUME_MIN = 0.0;
    const double VOLUME_MAX = 1.0;
    private double _volume = 0.5;
    public double Volume
    {
        get { return _volume; }
        set
        {
            if (_volume != Math.Clamp(value, VOLUME_MIN, VOLUME_MAX))
            {
                SetProperty(ref _volume, Math.Clamp(value, VOLUME_MIN, VOLUME_MAX));
            }
        }

    } //Volume

    const double PITCH_MIN = 0.01;
    const double PITCH_MAX = 2.0;
    private double _pitch;
    public double Pitch
    {
        get { return _pitch; }
        set
        {
            if (_pitch != Math.Clamp(value, PITCH_MIN, PITCH_MAX))
            {
                SetProperty(ref _pitch, Math.Clamp(value, PITCH_MIN, PITCH_MAX));
            }
        }

    } //Pitch

    private string _textToSpeak;
    public string TextToSpeak
    {
        get { return _textToSpeak; }
        set { SetProperty(ref _textToSpeak, value); }
    
    } //TextToSpeak


    internal bool _initialized;
    public bool Initialized
    {
        get { return _initialized; }
        set { SetProperty(ref _initialized, value); }
    }


    /// <summary>
    /// 
    /// </summary>
    public TextToSpeechViewModel()
    {
        Initialized = false;
        InitializeLocalesListAsync();
        SpeakNowAsyncCommand = new Command(SpeakNowAsync, canExecute: () => { return !SpeakingNow; });

        Volume = 0.5f;
        Pitch = 1.0f;


        ((Command)SpeakNowAsyncCommand).ChangeCanExecute();

    } //TextToSpeechViewModel

    private Task InitializeLocalesListAsync()
    {
        return Task.Run(async () => {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //Debug.WriteLine("*** InitializeLocalesList: start...");
            IEnumerable<Locale> locales = await TextToSpeech.GetLocalesAsync();
            List<Locale> localesList = new List<Locale>();
            foreach (Locale locale in locales)
            {
                //Debug.WriteLine(locale.Name);
                localesList.Add(locale);
            }
            localesList.Sort(new Comparison<Locale>((x, y) => String.Compare(x.Name, y.Name)));
            LocalesList = localesList;
            try
            {
                SelectedLocale = LocalesList.Single(locale => locale.Name == "English (United States)");
            }
            catch
            {
                SelectedLocale = LocalesList.FirstOrDefault();
            }
            Initialized = true;
            Debug.WriteLine("*** InitializeLocalesList: completed in {0:N} seconds", sw.ElapsedMilliseconds / 1000);

        });
    }

    public async void SpeakNowAsync()
    {

        if (!Initialized) 
        { 
            return; 
        } 
        
        var speechOptions = new SpeechOptions()
        {
            Volume = (float)Volume,
            Pitch = (float)Pitch,
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
}
