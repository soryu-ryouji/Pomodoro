using System.Windows;
using System.Windows.Input;
using Pomodoro.ViewModel;

namespace Pomodoro;

public partial class PomodoroView : Window
{
    private PomodoroViewModel _pomodoroViewModel;

    public PomodoroView()
    {
        InitializeComponent();
        _pomodoroViewModel = PomodoroViewModel.Instance;

        this.DataContext =_pomodoroViewModel;
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

    private void TimeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_pomodoroViewModel.IsRunning)
        {
            if (_pomodoroViewModel.IsCountDowning) _pomodoroViewModel.PauseCountDown();
            else _pomodoroViewModel.ResumeCountDown();
        }
        else
        {
            _pomodoroViewModel.StartWork();
        }
    }

    private void TimeButton_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        
    }
}