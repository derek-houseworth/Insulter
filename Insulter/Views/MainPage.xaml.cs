using Insulter.ViewModels;

namespace Insulter.Views;

public partial class MainPage : ContentPage
{
    private Label? _labelInsult = null;

    /// <summary>
    /// initializes user interface and hooks up view model, speaks welcome message
    /// </summary>
    public MainPage()
	{
		InitializeComponent();

        //update view model with UI method to be called when speaking complete
        ((InsulterViewModel)BindingContext).SpeakingComplete += OnInsultSpoken;

        DisplayInsults();
        UpdateInsultStatus(false);

        //timer to delay speaking welcome message at index 0 of insults list until 1 second after app startup
        Application.Current?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
			{
				if (((InsulterViewModel)BindingContext).Initialized)
				{
					SpeakInsult((Label)stackLayoutInsults.Children[0]);
				}

				//terminate timer after intro phrase has been spoken
				return !((InsulterViewModel)BindingContext).Initialized;
			});

	} //MainPage


	/// <summary>
	/// called by view model when speaking complete to update user interface by 
    /// removing most recently spoken insult and resetting status of remaining insults
	/// </summary>
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


    /// <summary>
    /// toggles color and enabled status of insults labels at beginning and end of speaking, i.e. 
    /// disable all insults while speaking and enable remaining when speaking complete
    /// /// </summary>
    /// <param name="insultsEnabled"></param>
    private void UpdateInsultStatus(bool insultsEnabled)
    {
        foreach (Label labelInsult in stackLayoutInsults.Children.Cast<Label>())
        {
            labelInsult.IsEnabled = insultsEnabled;
            labelInsult.TextColor = insultsEnabled ? Color.FromRgb(72,72,72) : Color.FromRgb(157,157,157);
        }

	} //UpdateInsultStatus


	/// <summary>
	/// creates label for each insult and adds to main stack layout
	/// </summary>
	private void DisplayInsults()
    {
        stackLayoutInsults.Children.Clear();

        foreach (string insult in ((InsulterViewModel)BindingContext).InsultsList)
        {
            stackLayoutInsults.Children.Add(GetInsultLabel(insult));
        }

	} //DisplayInsults


	private Label GetInsultLabel(string insultText)
    {
        Label insultLabel = new() 
        {
            Text = insultText,
            FontSize = DeviceInfo.Idiom == DeviceIdiom.Phone ? 42 : 62, 
            TextColor = Color.FromRgb(0, 255, 0),
            FontFamily = "BlackAdderITCRegular"
        };
		TapGestureRecognizer tgr = new();
		tgr.Tapped += OnInsultTapped;
		insultLabel.GestureRecognizers.Add(tgr);

		return insultLabel;

	} //GetInsultLabel


	/// <summary>
	/// updates user interface to highlight selected insult currently being spoken and disable 
	/// all other insults then calls view model to speak insult at specified index in 
	/// view model's insults list
	/// </summary>
	/// <param name="label"></param>
	private void SpeakInsult(Label label)
    {
        _labelInsult = label;
        UpdateInsultStatus(false);
        _labelInsult.TextColor = Color.FromRgb(255, 127, 39);
        _labelInsult.FontAttributes = FontAttributes.Italic;

        int indexInsult = stackLayoutInsults.Children.IndexOf(_labelInsult);
        ((InsulterViewModel)BindingContext).SpeakInsult(indexInsult);

	} //SpeakInsult


	/// <summary>
	/// handles tapped event for each label containing insults
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnInsultTapped(object? sender, EventArgs e)
    {

        if (!((InsulterViewModel)BindingContext).SpeakingNow && sender is not null)
        {
            SpeakInsult((Label)sender);
        }

	} //OnInsultTapped

} //MainPage