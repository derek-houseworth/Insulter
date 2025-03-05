using Insulter.ViewModels;
using Microsoft.Extensions.Logging;

namespace Insulter
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
                    fonts.AddFont("BlackAdderITCRegular.ttf", "BlackAdderITCRegular");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			builder.Services.AddSingleton<Views.MainPage>();
			builder.Services.AddSingleton<InsulterViewModel>();

			return builder.Build();
		}
	}
}
