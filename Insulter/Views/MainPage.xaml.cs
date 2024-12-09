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

        //update view model with MainPage method to be called after selected insult has been spoken
        ((InsulterViewModel)BindingContext).SpeakingComplete += OnInsultSpoken;

        DisplayInsults();

        //timer to delay speaking welcome message at index 0 of insults list until 1 second after app startup
        Application.Current?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
		{
			if (((InsulterViewModel)BindingContext).Initialized)
			{
				_selectedInsultLabel = (Label)stackLayoutInsults.Children[0];
				OnInsultTapped(_selectedInsultLabel, new EventArgs());
			}

			//terminate timer after speaking of intro phrase has started
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

		//load insults if all insults have been spoken and stackLayout is empty
		if (stackLayoutInsults.Count == 0)
		{
			DisplayInsults();
		}

    } //OnInsultSpoken


	/// <summary>
	/// creates label for each insult and adds to main stack layout
	/// </summary>
	private void DisplayInsults()
    {
        stackLayoutInsults.Children.Clear();

		foreach (string insultText in ((InsulterViewModel)BindingContext).InsultsList)
        {
			TapGestureRecognizer tgr = new();
			tgr.Tapped += OnInsultTapped;
			Label insultLabel = new()
			{
				FontSize = DeviceInfo.Idiom == DeviceIdiom.Phone ? 42 : 54,
				Style = (Style)this.Resources["insultNotSpeakingStyle"],
				Text = insultText
			};
			insultLabel.GestureRecognizers.Add(tgr);
			insultLabel.SetBinding(Label.IsEnabledProperty, "CanSpeak");
			stackLayoutInsults.Children.Add(insultLabel);
        }

	} //DisplayInsults


	
	/// <summary>
	/// common handler of Tapped event for all insult labels
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnInsultTapped(object? sender, EventArgs e)
    {

        if (((InsulterViewModel)BindingContext).CanSpeak && sender is not null)
        {
			_selectedInsultLabel = (Label)sender;

			//change style of selected insult label to indicate it is being spoken
			_selectedInsultLabel.Style = (Style)this.Resources["insultSpeakingStyle"];

			//speak text contents of selected insult label
			int indexSelectedInsult = stackLayoutInsults.Children.IndexOf(_selectedInsultLabel);
			((InsulterViewModel)BindingContext).SpeakInsult(indexSelectedInsult);
		}

	} //OnInsultTapped
	

} //MainPage