using Insulter.ViewModels;
using Insulter.Views;

namespace Insulter;

/// <summary>
/// inherited window class to enable responses to app lifecycle events, e.g. 
/// call main page view model's save state method on app shutdown or deactivation
/// </summary>
public class InsulterWindow : Window
{

	private readonly MainPage _mainPage;

	public InsulterWindow() : base() { }
	public InsulterWindow(MainPage page) : base(page)
	{
		_mainPage = page;
	}

	//called on app launch before main page visible
	protected override void OnCreated()
	{
	}

	/// <summary>
	/// called when app loses focus
	/// </summary>
	protected override void OnDeactivated()
	{
		((InsulterViewModel)_mainPage.BindingContext).SaveState();
	}
	
	/// <summary>
	/// called when app is shuttig down
	/// </summary>
	protected override void OnDestroying()
	{
		((InsulterViewModel)_mainPage.BindingContext).SaveState();
	}

} //InsulterWindow

