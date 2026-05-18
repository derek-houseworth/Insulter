using System.Reflection;
using Insulter.ViewModels;

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
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");


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
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentNullException>(() => new TextToSpeechViewModel(null!, new MockPreferencesService()));
            Assert.Throws<ArgumentNullException>(() => new TextToSpeechViewModel(new MockTtsService(), null!));
        }

    } //TestNullServiceArgsInConstructor


    [Test]
    public void TestVoiceSelection()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        var viewModel = new TextToSpeechViewModel(new MockTtsService(), new MockPreferencesService())
        {
            SelectedVoice = "es (es-ESSpanish (Spain))"
        };
            
        Assert.That(viewModel.SelectedVoice, Is.EqualTo("es (es-ESSpanish (Spain))"));
        

    } //TestVoiceSelection

    [Test]
    public void TestVolumeAndPitch()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");
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
    public void TestAutoSave()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        var mockTtsService = new MockTtsService();
        var mockPrefsService = new MockPreferencesService();
        var viewModel = new TextToSpeechViewModel(mockTtsService, mockPrefsService)
        {
            AutoSave = true
        };

        //change property values from defaults to trigger the auto-save functionality
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

    } //TestAutoSave


    [Test]
    public void TestLoadPropertyValuesFromPreferences()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

        var mockTtsService = new MockTtsService();
        var mockPrefsService = new MockPreferencesService();
        var viewModel1 = new TextToSpeechViewModel(mockTtsService, mockPrefsService)
        {
            AutoSave = true,
            SelectedVoice = "fr (fr-FRFrench (France))",
            Volume = 0.7f,
            Pitch = 1.1f
        };
        viewModel1.SaveState();


        // Create a new instance of the view model to verify that it loads the property values from the preferences service
        var viewModel2 = new TextToSpeechViewModel(mockTtsService, mockPrefsService);
        Task.Delay(500).Wait();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel2.SelectedVoice, Is.EqualTo("fr (fr-FRFrench (France))"));
            Assert.That(viewModel2.Volume, Is.EqualTo(0.7f));
            Assert.That(viewModel2.Pitch, Is.EqualTo(1.1f));
        }


    } //TestLoadPropertyValuesFromPreferences

} //TextToSpeechViewModelTests