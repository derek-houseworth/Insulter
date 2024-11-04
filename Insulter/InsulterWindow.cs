using Insulter.ViewModels;
using Insulter.Views;

namespace Insulter;

/// <summary>
/// inherited window class to enable responses to app lifecycle events, e.g. 
/// call main page view model's save state method on app shutdown or deactivation
/// </summary>
public class InsulterWindow : Window
{

	private readonly MainPage? _mainPage;

	public InsulterWindow() : base() { }
	public InsulterWindow(MainPage page) : base(page)
	{

		_mainPage = page;

	} //InsulterWindow


	/// <summary>
	/// called on app launch before main page visible
	/// </summary>
	protected override void OnCreated()
	{
	} //OnCreated


	/// <summary>
	/// called when app loses focus
	/// </summary>
	protected override void OnDeactivated()
	{

		if (_mainPage is not null)
		{
			((InsulterViewModel)_mainPage.BindingContext).SaveState();
		}

	} //OnDeactivated


	/// <summary>
	/// called when app is shutting down
	/// </summary>
	protected override void OnDestroying()
	{
		if (_mainPage is not null)
		{
			((InsulterViewModel)_mainPage.BindingContext).SaveState();
		}

	} //OnDestroying

} //InsulterWindow