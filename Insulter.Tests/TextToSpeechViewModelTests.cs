using System.Reflection;
using Insulter.ViewModels;

namespace Insulter.Tests
{
	public class TextToSpeechViewModelTests
	{
		[SetUp]
		public void Setup()
		{
		}


		[Test]
		public void TestInitialState()
		{
			TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			var viewModel = new TextToSpeechViewModel();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(viewModel, Is.Not.Null);
				Assert.That(viewModel.Voices, Has.Count.EqualTo(0));
				Assert.That(viewModel.SelectedVoice, Is.EqualTo(string.Empty));
				Assert.That(viewModel.Initialized, Is.False);
				Assert.That(viewModel.CanSpeak, Is.False);
				Assert.That(viewModel.AutoSave, Is.False);
				Assert.That(viewModel.SpeakNow.CanExecute(null), Is.False);
			}

		} //TestInitialState

	} //TextToSpeechViewModelTests

} //Insulter.Tests
