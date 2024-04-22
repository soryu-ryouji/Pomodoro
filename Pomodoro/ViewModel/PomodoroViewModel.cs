using System.ComponentModel;
using System.Windows.Threading;

namespace Pomodoro.ViewModel;

public class PomodoroViewModel: INotifyPropertyChanged
{
    public PomodoroViewModel()
    {
        RemainingTime = WorkTime;
    }
    private static readonly object s_lockObject = new object();
    private static PomodoroViewModel s_instance;

    public static PomodoroViewModel Instance
    {
        get
        {
            lock (s_lockObject)
            {
                return s_instance ??= new PomodoroViewModel();
            }
        }
    }

    private enum PomodoroState
    {
        Work,
        Break
    }

    private PomodoroState _state = PomodoroState.Work;
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private DispatcherTimer _timer = new();

    public bool IsRunning = false;
    public bool IsCountDowning { get; private set; } = false;

    public TimeSpan WorkTime { get; set; } = TimeSpan.FromMinutes(25);
    public TimeSpan BreakTime { get; set; } = TimeSpan.FromMinutes(5);

    private TimeSpan _remainingTime = TimeSpan.FromMinutes(0);

    private TimeSpan RemainingTime
    {
        get => _remainingTime;
        set
        {
            _remainingTime = value;
            var temp = _remainingTime.ToString(@"mm\:ss");
            RemainingMinutes = temp[..2];
            RemainingSeconds = temp[3..];
        }
    }

    private string _remainingMinutes;

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

    private string _remainingSeconds;

    public string RemainingSeconds
    {
        get => _remainingSeconds;
        private set
        {
            _remainingSeconds = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingSeconds)));
        }
    }

    private void StartCountDown(TimeSpan countDownTime)
    {
        _timer.Tick += CountDown;
        _timer.Interval = TimeSpan.FromSeconds(1);
        RemainingTime = countDownTime;
        IsRunning = true;
        IsCountDowning = true;
        _timer.Start();
    }

    private void CountDown(object? sender, EventArgs e)
    {
        SubtractRemainingTime(TimeSpan.FromSeconds(1));

        if (RemainingTime.TotalSeconds <= 0) FinishCountDown();
    }

    private void SubtractRemainingTime(TimeSpan sub)
    {
        RemainingTime -= sub;
    }

    public void StartWork()
    {
        _state = PomodoroState.Work;
        StartCountDown(WorkTime);
    }

    public void StartBreak()
    {
        _state = PomodoroState.Break;
        StartCountDown(BreakTime);
    }

    private void FinishCountDown()
    {
        PauseCountDown();

        if (_state == PomodoroState.Work)
        {
            var finishedWorkView = new FinishedWorkView();
            finishedWorkView.Show();
        }
        else if (_state == PomodoroState.Break)
        {
            var finishedBreakView = new FinishedBreakView();
            finishedBreakView.Show();
        }
    }

    public void InitView()
    {
        RemainingTime = WorkTime;
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