using Insulter.ViewModels;

namespace Insulter.Views;

public partial class MainPage : ContentPage
{
    private Label _labelInsult = null;

    InsulterViewModel _insulterViewModel;

    public MainPage(InsulterViewModel viewModel)
	{
		InitializeComponent();

        _insulterViewModel = viewModel;
        _insulterViewModel.SpeakingComplete += OnInsultSpoken;
        BindingContext = _insulterViewModel;


        DisplayInsults();
        UpdateInsultStatus(false);

        Application.Current.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
        {
            if (_insulterViewModel.Initialized)
            {
                SpeakInsult((Label)stackLayoutInsults.Children[0]);
            }
            return !_insulterViewModel.Initialized;
        });

    }

    private void OnInsultSpoken()
    {

        int indexSpokenInsult = stackLayoutInsults.Children.IndexOf(_labelInsult);
        stackLayoutInsults.Children.RemoveAt(indexSpokenInsult);
        _labelInsult = null;
        if (stackLayoutInsults.Children.Count > 0)
        {
            UpdateInsultStatus(true);
        }
        else
        {
            DisplayInsults();
        }

    } //OnInsultSpoken


    private void UpdateInsultStatus(bool insultsEnabled)
    {
        foreach (Label labelInsult in stackLayoutInsults.Children)
        {
            labelInsult.IsEnabled = insultsEnabled;
            labelInsult.TextColor = insultsEnabled ? Color.FromRgb(72,72,72) : Color.FromRgb(157,157,157);
        }
    }

    private void DisplayInsults()
    {
        stackLayoutInsults.Children.Clear();

        foreach (string insult in _insulterViewModel.InsultsList)
        {
            stackLayoutInsults.Children.Add(GetInsultLabel(insult));
        }

    }

    private Label GetInsultLabel(string insultText)
    {
        Label insultLabel = new Label
        {
            Text = insultText,
            FontSize = DeviceInfo.Idiom == DeviceIdiom.Phone ? 42 : 62, 
            //FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromRgb(0, 255, 0),
            FontFamily = "BlackAdderITCRegular"
        };
        TapGestureRecognizer tgr = new();
        tgr.Tapped += OnInsultTapped;
        insultLabel.GestureRecognizers.Add(tgr);

        return insultLabel;

    }

    private void SpeakInsult(Label label)
    {
        _labelInsult = label;
        UpdateInsultStatus(false);
        _labelInsult.TextColor = Color.FromRgb(255, 127, 39);
        _labelInsult.FontAttributes = /*FontAttributes.Bold |*/ FontAttributes.Italic;

        int indexInsult = stackLayoutInsults.Children.IndexOf(_labelInsult);
        _insulterViewModel.SpeakInsult(indexInsult);
    }

    private void OnInsultTapped(object sender, EventArgs e)
    {
        if (!_insulterViewModel.SpeakingNow)
        {
            SpeakInsult((Label)sender);
        }
    }


}

