using Insulter.Services;

namespace Insulter.Models
{
    public class Insults
    {

        public List<string> InsultsList { get => _insultsService.GetInsultList();  }

        private InsultBuilderService _insultsService;

        public Insults(string adjectivesFile, string adverbsFile, string nounsFile) 
        {

            _insultsService = new InsultBuilderService(adjectivesFile, adverbsFile, nounsFile);

        } //Insults


    } //Insults

} //Insulter.Models