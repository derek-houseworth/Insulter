using Insulter.ViewModels;
using System.Reflection;
using Insulter.Tests.Services;

namespace Insulter.Tests;

public class InsulterViewModelTests
{
	private const int TOTAL_INSULTS_COUNT = 50;

    [SetUp]
	public void Setup()
	{
	}

	[Test]
	public void TestInitialState()
	{
		TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");

		var viewModel = new InsulterViewModel(new MockTtsService(), new MockPreferencesService());
        using (Assert.EnterMultipleScope())
        {
			Assert.That(viewModel, Is.Not.Null);
			Assert.That(viewModel.InsultsList, Has.Count.EqualTo(TOTAL_INSULTS_COUNT+1));
			Assert.That(viewModel.InsultsSpoken, Is.Zero);
            Assert.That(viewModel.CanSpeak, Is.True);
            Assert.That(viewModel.Initialized, Is.True);
			Assert.That(viewModel.SelectedInsult, Is.Empty);
		}

    } //TestInitialState


    [Test]
    public void TestSpeakAllInsults()
    {
        TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");
        
		var viewModel = new InsulterViewModel(new MockTtsService(), new MockPreferencesService());
        using (Assert.EnterMultipleScope())
        {
            int insultsSpoken = 0;
            while (insultsSpoken < TOTAL_INSULTS_COUNT+1)
			{
                Assert.That(viewModel.InsultsList, Has.Count.EqualTo(TOTAL_INSULTS_COUNT+1-insultsSpoken));
                viewModel.SelectedInsult = viewModel.InsultsList[0];
                viewModel.SpeakNow.Execute(viewModel.SelectedInsult);
                /*
                while (!viewModel.CanSpeak)
                {
                    Task.Delay(25).Wait();
                }
                */
                insultsSpoken++;
                Assert.That(viewModel.InsultsSpoken, Is.EqualTo(insultsSpoken));
                
            }
            Assert.That(viewModel.InsultsList,Has.Count.EqualTo(TOTAL_INSULTS_COUNT));
        }


    } //TestSpeakAllInsults

} //InsulterViewModelTests
