using Microsoft.Win32;
using DeckGlow.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace DeckGlow.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;
            _viewModel.DeleteAppEvent += ShowAppDeletedSnackbar;
            InitializeComponent();
        }

        private void ShowAppDeletedSnackbar(AppItemViewModel appItemViewModel)
        {
            if (AppDeletedSnackbar.MessageQueue is not { } messageQueue) return;

            Task.Factory.StartNew(() => messageQueue.Enqueue(
                string.Format("{0} removed", appItemViewModel.AppName),
                "UNDO",
                () => _viewModel.AddApp(appItemViewModel.AppPath, appItemViewModel.Brightness)
            ));
        }

        private void AddApp_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Title = "Choose app",
                DefaultExt = ".exe",
                Filter = "All (*.exe)|*.exe"
            };
            bool result = dialog.ShowDialog() ?? false;
            if (!result) return;

            _viewModel.AddApp(dialog.FileName, 100);
        }

        private void SliderDefaultBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _viewModel.SetDefaultBrightness((int)((Slider)sender).Value, true);
        }

        private void SliderDefaultBrightness_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.SetDefaultBrightness((int)((Slider)sender).Value, true);
        }

        private void ButtonVersion_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(App.UrlReleaseHistory) { UseShellExecute = true });
        }
    }
}
