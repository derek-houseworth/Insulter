using Insulter.Tests.Services;
using Insulter.ViewModels;
using System.Reflection;

namespace Insulter.Tests;

public class TextToSpeechViewModelTests
{

    private const string DEFAULT_VOICE = "Microsoft David (en-US)";
    private const string TEST_VOICE = "Microsoft Zira (en-US)";

    private const float DEFAULT_PITCH = 1.0f;
    private const float TEST_PITCH = 1.4f;

    private const float DEFAULT_VOLUME = 0.5f;
    private const float TEST_VOLUME = 0.6f;


    [Test]
    public void TestInitialState()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");


        var viewModel = new TextToSpeechViewModel(new MockTtsService(), new MockPreferencesService());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.Voices, Has.Count.GreaterThan(0));

            Assert.That(viewModel.SelectedVoice, Is.EqualTo(DEFAULT_VOICE));
            Assert.That(viewModel.Initialized, Is.True);
            Assert.That(viewModel.CanSpeak, Is.True);
            Assert.That(viewModel.AutoSave, Is.True);
            Assert.That(viewModel.SpeakNow.CanExecute(null), Is.True);
            Assert.That(viewModel.Pitch, Is.EqualTo(DEFAULT_PITCH));
            Assert.That(viewModel.Volume, Is.EqualTo(DEFAULT_VOLUME));

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
            SelectedVoice = TEST_VOICE
        };
            
        Assert.That(viewModel.SelectedVoice, Is.EqualTo(TEST_VOICE));
        

    } //TestVoiceSelection

    [Test]
    public void TestVolumeAndPitch()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");
        var viewModel = new TextToSpeechViewModel(new MockTtsService(), new MockPreferencesService())
        {
            Volume = TEST_VOLUME,
            Pitch = TEST_PITCH
        };
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.Volume, Is.EqualTo(TEST_VOLUME));
            Assert.That(viewModel.Pitch, Is.EqualTo(TEST_PITCH));
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
            SelectedVoice = TEST_VOICE,
            Volume = TEST_VOLUME,
            Pitch = TEST_PITCH
        };

        //verify property values were saved to the preferences service
        using (Assert.EnterMultipleScope())
        {
            Assert.That(mockPrefsService.ContainsKey("SelectedVoice"), Is.True);
            Assert.That(mockPrefsService.Get("SelectedVoice", string.Empty), Is.EqualTo(TEST_VOICE));
            Assert.That(mockPrefsService.ContainsKey("Volume"), Is.True);
            Assert.That(mockPrefsService.Get("Volume", string.Empty), Is.EqualTo(TEST_VOLUME.ToString()));
            Assert.That(mockPrefsService.ContainsKey("Pitch"), Is.True);
            Assert.That(mockPrefsService.Get("Pitch", string.Empty), Is.EqualTo(TEST_PITCH.ToString()));
        }

    } //TestSaveState

    private const string APP_SETTINGS_VOICE_KEY = "SelectedVoice";
    private const string APP_SETTINGS_VOLUME_KEY = "Volume";
    private const string APP_SETTINGS_PITCH_KEY = "Pitch";

    [Test]
    public void TestRestoreState()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");


        var mps = new MockPreferencesService();
        mps.Set(APP_SETTINGS_VOICE_KEY, TEST_VOICE);
        mps.Set(APP_SETTINGS_VOLUME_KEY, TEST_VOLUME.ToString());
        mps.Set(APP_SETTINGS_PITCH_KEY, TEST_PITCH.ToString());

        var viewModel = new TextToSpeechViewModel(new MockTtsService(), mps);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.SelectedVoice, Is.EqualTo(TEST_VOICE));
            Assert.That(viewModel.Volume, Is.EqualTo(TEST_VOLUME));
            Assert.That(viewModel.Pitch, Is.EqualTo(TEST_PITCH));
        }


    } //TestRestoreState

} //TextToSpeechViewModelTests