using Insulter.Services;
using System.Reflection;
namespace Insulter.Tests
{
	public class InsultBuilderTests
	{

		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void TestGetInsult()
		{
			TestHelper.DebugWriteLine($"{this.GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			var insult = InsultBuilder.GetInsult();
			TestHelper.DebugWriteLine(insult);
			Assert.That(string.IsNullOrEmpty(insult), Is.False);

		} //TestGetInsult


		[Test]
		public void TestGetInsults()
		{
			TestHelper.DebugWriteLine($"{this.GetType().Name}.{MethodBase.GetCurrentMethod()}:");

			//generate insults list
			var insultsList = InsultBuilder.GetInsults();
			Assert.That(insultsList, Is.Not.Null);

			Assert.Multiple(() =>
			{
				//verify list not empty 
				Assert.That(insultsList, Has.Count.GreaterThan(0));

				//verify all list elements unique
				HashSet<string> uniqueList = new(insultsList);
				Assert.That(insultsList, Has.Count.EqualTo(uniqueList.Count));

				foreach (var insult in insultsList)
				{
					Assert.That(string.IsNullOrEmpty(insult), Is.False);
					TestHelper.DebugWriteLine(insult);
				}
			});

		} //TestGetInsults

	} //InsultBuilderTests

} //Insulter.Tests