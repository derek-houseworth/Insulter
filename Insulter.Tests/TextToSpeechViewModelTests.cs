using System.Reflection;
using Insulter.ViewModels;
using Insulter.Tests.Services;

namespace Insulter.Tests;

public class TextToSpeechViewModelTests
{

    [SetUp]
    public void Setup()
    {
    } //Setup


    [Test]
    public void TestInitialState()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");


        var viewModel = new TextToSpeechViewModel(new MockTtsService(), new MockPreferencesService());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.Voices, Has.Count.GreaterThan(0));
            Assert.That(viewModel.SelectedVoice, Is.EqualTo("de (de-DEGerman (Germany))"));
            Assert.That(viewModel.Initialized, Is.True);
            Assert.That(viewModel.CanSpeak, Is.True);
            Assert.That(viewModel.AutoSave, Is.False);
            Assert.That(viewModel.SpeakNow.CanExecute(null), Is.True);
            Assert.That(viewModel.Pitch, Is.EqualTo(1.0f));
            Assert.That(viewModel.Volume, Is.EqualTo(0.5f));

        }

    } //TestInitialState


    [Test]
    public void TestNullServiceArgsInConstructor()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");

        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentNullException>(() => new TextToSpeechViewModel(null!, new MockPreferencesService()));
            Assert.Throws<ArgumentNullException>(() => new TextToSpeechViewModel(new MockTtsService(), null!));
        }

    } //TestNullServiceArgsInConstructor


    [Test]
    public void TestVoiceSelection()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");

        var viewModel = new TextToSpeechViewModel(new MockTtsService(), new MockPreferencesService())
        {
            SelectedVoice = "es (es-ESSpanish (Spain))"
        };
            
        Assert.That(viewModel.SelectedVoice, Is.EqualTo("es (es-ESSpanish (Spain))"));
        

    } //TestVoiceSelection

    [Test]
    public void TestVolumeAndPitch()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");
        var viewModel = new TextToSpeechViewModel(new MockTtsService(), new MockPreferencesService())
        {
            Volume = 0.8f,
            Pitch = 1.2f
        };
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.Volume, Is.EqualTo(0.8f));
            Assert.That(viewModel.Pitch, Is.EqualTo(1.2f));
        }
    } //TestVolumeAndPitch


    [Test]
    public void TestSaveState()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");

        var mockTtsService = new MockTtsService();
        var mockPrefsService = new MockPreferencesService();
        var viewModel = new TextToSpeechViewModel(mockTtsService, mockPrefsService)
        {
            AutoSave = true
        };

        //change property values from defaults after instantiation to trigger  viewmodel's auto-save functionality
        viewModel.SelectedVoice = "fr (fr-FRFrench (France))";
        viewModel.Volume = 0.7f;
        viewModel.Pitch = 1.1f;

        //verify property values were saved to the preferences service
        using (Assert.EnterMultipleScope())
        {
            Assert.That(mockPrefsService.ContainsKey("SelectedVoice"), Is.True);
            Assert.That(mockPrefsService.Get("SelectedVoice", string.Empty), Is.EqualTo("fr (fr-FRFrench (France))"));
            Assert.That(mockPrefsService.ContainsKey("Volume"), Is.True);
            Assert.That(mockPrefsService.Get("Volume", string.Empty), Is.EqualTo("0.7"));
            Assert.That(mockPrefsService.ContainsKey("Pitch"), Is.True);
            Assert.That(mockPrefsService.Get("Pitch", string.Empty), Is.EqualTo("1.1"));
        }

    } //TestSaveState

    private const string APP_SETTINGS_VOICE_KEY = "SelectedVoice";
    private const string APP_SETTINGS_VOLUME_KEY = "Volume";
    private const string APP_SETTINGS_PITCH_KEY = "Pitch";

    [Test]
    public void TestRestoreState()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");

        string selectedVoice = "fr (fr-FRFrench (France))";
        double volume = 0.70f, pitch = 1.10f;

        var mps = new MockPreferencesService();
        mps.Set(APP_SETTINGS_VOICE_KEY, selectedVoice);
        mps.Set(APP_SETTINGS_VOLUME_KEY, volume.ToString());
        mps.Set(APP_SETTINGS_PITCH_KEY, pitch.ToString());

        var viewModel = new TextToSpeechViewModel(new MockTtsService(), mps);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.SelectedVoice, Is.EqualTo(selectedVoice));
            Assert.That(viewModel.Volume, Is.EqualTo(volume));
            Assert.That(viewModel.Pitch, Is.EqualTo(pitch));
        }


    } //TestRestoreState

} //TextToSpeechViewModelTests