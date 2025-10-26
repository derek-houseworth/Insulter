using Insulter.ViewModels;
using System.Reflection;

namespace Insulter.Tests
{
	public class InsulterViewModelTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void TestInitialState()
		{
			TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			var viewModel = new InsulterViewModel();
            using (Assert.EnterMultipleScope())
            {
				Assert.That(viewModel, Is.Not.Null);
				Assert.That(viewModel.InsultsList, Has.Count.GreaterThan(0));
				Assert.That(viewModel.InsultsSpoken, Is.EqualTo(0));
				Assert.That(viewModel.Initialized, Is.False);
			}
		}
	}
}
