using Insulter.ViewModels;

namespace Insulter.Views;

public partial class MainPage : ContentPage
{
    private Label? _selectedInsultLabel = null;

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
					_selectedInsultLabel = (Label)stackLayoutInsults.Children[0];
					SpeakInsult();
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

		//remove label for selected (spoken) insult from stack layout
		int indexSelectedInsult = stackLayoutInsults.Children.IndexOf(_selectedInsultLabel);
        stackLayoutInsults.Children.RemoveAt(indexSelectedInsult);
		_selectedInsultLabel = null;

		//enable remaining insults to make them clickable
		UpdateInsultStatus(true);

		//load insults if all insults have been spoken and stackLayout is empty
		if (stackLayoutInsults.Count == 0)
		{
			DisplayInsults();
		}

    } //OnInsultSpoken


    /// <summary>
    /// toggles  enabled status of insults labels at beginning and end of speaking, i.e. 
    /// disable all insults while speaking and enable remaining after speaking complete
    /// /// </summary>
    /// <param name="insultsEnabled"></param>
    private void UpdateInsultStatus(bool insultsEnabled)
    {
        foreach (Label labelInsult in stackLayoutInsults.Children.Cast<Label>())
        {
			labelInsult.IsEnabled = insultsEnabled;
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
		TapGestureRecognizer tgr = new();
		tgr.Tapped += OnInsultTapped;
		Label insultLabel = new() 
        {
            Text = insultText,
            FontSize = DeviceInfo.Idiom == DeviceIdiom.Phone ? 42 : 54, 
            Style = (Style)this.Resources["insultNotSpeakingStyle"],
		};
		insultLabel.GestureRecognizers.Add(tgr);

		return insultLabel;

	} //GetInsultLabel


	/// <summary>
	/// updates user interface to highlight selected insult currently being spoken and disable 
	/// all other insults then calls view model to speak insult at specified index in 
	/// view model's insults list
	/// </summary>
	private void SpeakInsult()
    {
		if (_selectedInsultLabel is not null)
		{
			//disable all insult labels
			UpdateInsultStatus(false);
			//VisualStateManager.GoToState(_selectedInsultLabel, "Speaking");

			//change style of selected insult label to indicate it is being spoken
			_selectedInsultLabel.Style = (Style)this.Resources["insultSpeakingStyle"];

			//speak text contents of selected insult label
			int indexSelectedInsult = stackLayoutInsults.Children.IndexOf(_selectedInsultLabel);
			((InsulterViewModel)BindingContext).SpeakInsult(indexSelectedInsult);
		}

	} //SpeakInsult


	/// <summary>
	/// common handler of Tapped event for all insult labels
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnInsultTapped(object? sender, EventArgs e)
    {

        if (!((InsulterViewModel)BindingContext).SpeakingNow && sender is not null)
        {
			_selectedInsultLabel = (Label)sender;
			SpeakInsult();
        }

	} //OnInsultTapped

} //MainPage