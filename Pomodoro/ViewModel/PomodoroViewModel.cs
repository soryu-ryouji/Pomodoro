using System.ComponentModel;
using System.Windows.Threading;

namespace Pomodoro.ViewModel;

public class PomodoroViewModel: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private DispatcherTimer _timer = new();

    public bool IsRunning = false;
    public bool IsCountDowning { get; private set; } = false;

    public TimeSpan WorkTime { get; set; } = TimeSpan.FromMinutes(25);
    public TimeSpan BreakTime { get; set; } = TimeSpan.FromMinutes(5);

    private TimeSpan _remainingTime = TimeSpan.FromMinutes(0);


    public TimeSpan RemainingTime
    {
        get => _remainingTime;
        private set
        {
            _remainingTime = value;
            var temp = _remainingTime.ToString(@"mm\:ss");
            RemainingMinutes = temp[..2];
            RemainingSeconds = temp[3..];
        }
    }

    private string _remainingMinutes = "25";

    public string RemainingMinutes
    {
        get
        { 
            return _remainingMinutes;
        }
        private set
        {
            _remainingMinutes = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingMinutes)));
        }
    }

    private string _remainingSeconds = "00";

    public string RemainingSeconds
    {
        get
        {
            return _remainingSeconds;
        }
        private set
        {
            _remainingSeconds = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingSeconds)));
        }
    }

    public void StartCountDown(TimeSpan countDownTime)
    {
        _timer.Tick += new EventHandler(CountDown);
        _timer.Interval = TimeSpan.FromSeconds(1);
        RemainingTime = countDownTime;
        IsRunning = true;
        IsCountDowning = true;
        _timer.Start();
    }

    public void CountDown(object? sender, EventArgs e)
    {
        SubtractRemainingTime(TimeSpan.FromSeconds(1));

        if (RemainingTime.TotalSeconds <= 0) StopCountDown();
    }

    private void SubtractRemainingTime(TimeSpan sub)
    {
        RemainingTime -= sub;
    }

    public void StartWork()
    {
        StartCountDown(WorkTime);
    }

    public void StartBreak()
    {
        StartCountDown(BreakTime);
    }

    public void StopCountDown()
    {
        _timer.Stop();
        IsRunning = false;
    }

    public void PauseCountDown()
    {
        _timer.Stop();
        _timer.Tick -= CountDown;
        IsCountDowning = false;
    }

    public void ResumeCountDown()
    {
        IsCountDowning = true;
        _timer.Tick += CountDown;
        _timer.Start();
    }
}