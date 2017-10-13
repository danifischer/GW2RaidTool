using System;
using System.Windows;
using MahApps.Metro;

namespace RaidTool
{
   public partial class App : Application
    {
	    protected override void OnStartup(StartupEventArgs e)
	    {
		    // add custom accent and theme resource dictionaries to the ThemeManager
		    // you should replace MahAppsMetroThemesSample with your application name
		    // and correct place where your custom accent lives
		    ThemeManager.AddAccent("RaidToolAccent", new Uri("pack://application:,,,/RaidTool;component/RaidToolAccent.xaml"));

		    // get the current app style (theme and accent) from the application
		    Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);

		    // now change app style to the custom accent and current theme
		    ThemeManager.ChangeAppStyle(Application.Current,
			    ThemeManager.GetAccent("RaidToolAccent"),
			    theme.Item1);

		    base.OnStartup(e);
	    }
	}
}
