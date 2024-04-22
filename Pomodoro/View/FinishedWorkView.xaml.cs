using System.Windows;
using System.Windows.Input;
using Pomodoro.ViewModel;

namespace Pomodoro;

public partial class FinishedWorkView : Window
{
    public FinishedWorkView()
    {
        InitializeComponent();
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

    private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
    {
        PomodoroViewModel.Instance.InitView();
        Close();
    }

    private void StartBreakBtn_OnClick(object sender, RoutedEventArgs e)
    {
        PomodoroViewModel.Instance.StartBreak();
        Close();
    }
}