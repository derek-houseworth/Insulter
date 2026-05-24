using Insulter.Services;
using Insulter.ViewModels;

namespace Insulter.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		InitializeComponent();
        BindingContext = new InsulterViewModel(new TextToSpeechService(), new PreferencesService());


    } //MainPage

    private void InsultsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = (InsulterViewModel)BindingContext;
        if (vm.SelectedInsult is not null) 
        {
            var cv = (CollectionView)sender;
            cv.ScrollTo(vm.InsultsList.IndexOf(vm.SelectedInsult));
        }

    } //InsultsCollectionView_SelectionChanged

} //MainPage