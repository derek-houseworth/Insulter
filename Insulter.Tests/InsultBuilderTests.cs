using Insulter.Services;
using System.Reflection;
namespace Insulter.Tests;

public class InsultBuilderTests
{

	[SetUp]
	public void Setup()
	{
	}



	[Test]
	public void TestGetInsults()
	{
		TestHelper.DebugWriteLine($"{GetType().Name}.{MethodBase.GetCurrentMethod()?.Name}:");

		

        //generate insults list
        var insultsList = InsultBuilderService.GetInsults(Path.Combine(AppContext.BaseDirectory, "Testfiles")).Result;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(insultsList, Is.Not.Null);

            //verify list not empty 
            Assert.That(insultsList, Has.Count.GreaterThan(0));

			//verify all list elements unique
			HashSet<string> uniqueList = [.. insultsList];
			Assert.That(insultsList, Has.Count.EqualTo(uniqueList.Count));

			foreach (var insult in insultsList)
			{
				Assert.False(string.IsNullOrEmpty(insult));
                TestHelper.DebugWriteLine($"\t{insult}");
            }
        }

	} //TestGetInsults


} //InsultBuilderTests