using System.Diagnostics;
using System.Windows.Navigation;
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
    }
}
