using Insulter.ViewModels;

namespace Insulter.Views;

public partial class MainPage : ContentPage
{

    public MainPage()
	{
		InitializeComponent();

	} //MainPage

    private void InsultsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = (InsulterViewModel)BindingContext;
        if (vm.SelectedInsult is not null) 
        {
            var cv = (CollectionView)sender;
            cv.ScrollTo(vm.InsultsList.IndexOf(vm.SelectedInsult));
        }


    }
} //MainPage