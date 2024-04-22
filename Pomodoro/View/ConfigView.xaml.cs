using Pomodoro.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Pomodoro.View;

public partial class ConfigView : Window
{
    public ConfigView()
    {
        InitializeComponent();

        this.DataContext = PomodoroViewModel.Instance;
    }

    private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (this.WindowState == WindowState.Maximized)
        {
            this.WindowState = WindowState.Normal;
            this.DragMove();
        }
        else
        {
            this.DragMove();
        }
    }

    private void BackBtn_OnClick(object sender, RoutedEventArgs e)
    {
        PomodoroViewModel.Instance.InitView();
        PomodoroViewModel.Instance.SaveModel();
        Close();
    }
}