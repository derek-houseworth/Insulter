using Insulter.Services;

namespace Insulter.Models
{
    public class Insults
    {
		private const string DATA_FILE_PATH = "Insulter.Data.";
		private const string ADJECTIVES_FILE_NAME = DATA_FILE_PATH + "insultAdjectives.txt";
		private const string ADVERBS_FILE_NAME = DATA_FILE_PATH + "insultAdverbs.txt";
		private const string NOUNS_FILE_NAME = DATA_FILE_PATH + "insultNouns.txt";
		public List<string> InsultsList { get => _insultsService.GetInsultList();  }

        private readonly InsultBuilderService _insultsService;

        public Insults() 
        {

            _insultsService = new InsultBuilderService(ADJECTIVES_FILE_NAME, ADVERBS_FILE_NAME, NOUNS_FILE_NAME);

        } //Insults


    } //Insults

} //Insulter.Models