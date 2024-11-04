namespace Insulter;
using Insulter.Views;
using Insulter.ViewModels;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		//MainPage = new AppShell();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new InsulterWindow(new MainPage(new InsulterViewModel()));

		/*
		Window window = base.CreateWindow(activationState);

		window.Created += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 1. Created event");
		};
		window.Activated += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 2. Activated event");
		};
		window.Deactivated += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 3. Deactivated event");
		};
		window.Stopped += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 4. Stopped event");
		};
		window.Resumed += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 5. Resumed event");
		};
		window.Destroying += (s, e) => 
		{
			Debug.WriteLine("Insulter.App: 6. Destroying event");
		};

		return window;
		*/
	}
}
