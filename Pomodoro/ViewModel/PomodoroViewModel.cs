using System.ComponentModel;
using System.Windows.Threading;
using Pomodoro.Utils;

namespace Pomodoro.ViewModel;

public class PomodoroViewModel : INotifyPropertyChanged
{
    private PomodoroViewModel()
    {
        var configs = AssetManager.LoadConfig();

        if (configs.Count != 0)
        {
            WorkTime = TimeSpan.Parse(configs.FirstOrDefault(c => c.name == "Work Time").value.ToString() ?? "00:25");
            BreakTime = TimeSpan.Parse(configs.FirstOrDefault(c => c.name == "Break Time").value.ToString() ?? "00:05");
        }
        else
        {
            WorkTime = TimeSpan.FromMinutes(25);
            BreakTime = TimeSpan.FromMinutes(5);
        }

        RemainingTime = WorkTime;
    }

    private static PomodoroViewModel s_instance;

    private static bool s_inited;

    public static PomodoroViewModel Instance
    {
        get
        {
            if (s_inited is not true)
            {
                s_inited = true;
                s_instance = new PomodoroViewModel();
            }

            return s_instance;
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

    public bool IsRunning;
    public bool IsCountDowning { get; private set; }

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

    private string _remainingMinutes = "";

    public string RemainingMinutes
    {
        get => _remainingMinutes;
        private set
        {
            _remainingMinutes = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingMinutes)));
        }
    }

    private string _remainingSeconds = "";

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