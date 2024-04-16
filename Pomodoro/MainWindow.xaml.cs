using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Pomodoro
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private TimeSpan _remainingTime;
        private DispatcherTimer _timer = new();
        private bool _isPaused = false;

        private void ClockButton_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                if (!_isPaused)
                {
                    PauseCountDown();
                }
                else
                {
                    ResumeCountDown();
                }
            }
            else
            {
                if (!_isPaused)
                {
                    StartWork();
                }
                else
                {
                    ResumeCountDown();
                }
            }
        }

        private void StartWork()
        {
            _timer.Tick += new EventHandler(CountDown);
            _timer.Interval = TimeSpan.FromSeconds(1);
            _remainingTime = TimeSpan.FromMinutes(25);
            _timer.Start();
        }

        private void CountDown(object sender, EventArgs e)
        {
            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimeBlock(_remainingTime);

            if (_remainingTime.TotalSeconds <= 0) StopWork();
        }

        private void StopWork()
        {
            _timer.Stop();
            TimeBlock.Text = "25 : 00";
        }

        private void PauseCountDown()
        {
            _isPaused = true;
            _timer.Stop();
            _timer.Tick -= CountDown;
        }

        private void ResumeCountDown()
        {
            _isPaused = false;
            _timer.Tick += CountDown;
            _timer.Start();
        }

        private void UpdateTimeBlock(TimeSpan time)
        {
            TimeBlock.Text = time.ToString(@"mm\ \:\ ss");
        }
    }
}