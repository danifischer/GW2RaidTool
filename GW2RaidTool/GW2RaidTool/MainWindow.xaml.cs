using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using RaidTool.Helper;
using RaidTool.ViewModels;

namespace RaidTool
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow(MainViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}

		private static void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is PasswordBox passwordBox)
			{
				if (string.IsNullOrWhiteSpace(passwordBox.Password) == false)
				{
					Properties.Settings.Default.RaidarPassword = passwordBox.Password.Protect();
				}
				else
				{
					Properties.Settings.Default.RaidarPassword = string.Empty;
				}
				Properties.Settings.Default.Save();
			}
		}

		private void PasswordBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is PasswordBox passwordBox)
			{
				passwordBox.Password = string.IsNullOrEmpty(Properties.Settings.Default.RaidarPassword)
					? string.Empty
					: "123456789";
				passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
			}
		}
	}
}