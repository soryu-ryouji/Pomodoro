using System.ComponentModel;
using System.Globalization;
using System.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using Pomodoro.Utils;
using Pomodoro.View;

namespace Pomodoro.ViewModel;

public class PomodoroViewModel : INotifyPropertyChanged
{
    private PomodoroViewModel()
    {
        var configs = AssetManager.LoadConfig();

        if (configs.Count != 0)
        {
            WorkTime = int.Parse(configs.FirstOrDefault(c => c.name == "Work Time").value.ToString() ?? "25");
            BreakTime = int.Parse(configs.FirstOrDefault(c => c.name == "Break Time").value.ToString() ?? "5");
        }
        else
        {
            WorkTime = 25;
            BreakTime = 5;
        }

        RemainingTime = TimeSpan.FromMinutes(WorkTime);
    }

    public void SaveModel()
    {
        AssetManager.SaveConfig(
        [
            ("Work Time",WorkTime),
            ("Break Time",BreakTime)
        ]);
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

    public bool IsRunning = false;
    public bool IsCountDowning { get; private set; }

    private int _workTime;
    private int _breakTime;

    public int WorkTime
    {
        get => _workTime;
        set
        {
            if (_workTime == value) return;

            _workTime = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkTime)));
        }
    }

    public int BreakTime
    {
        get => _breakTime;
        set
        {
            if (_breakTime == value) return;

            _breakTime = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BreakTime)));
        }
    }

    private TimeSpan _remainingTime = TimeSpan.FromMinutes(0);

    private TimeSpan RemainingTime
    {
        get => _remainingTime;
        set
        {
            _remainingTime = value;
            RemainingMinutes = ((int)RemainingTime.TotalMinutes).ToString();
            RemainingSeconds = RemainingTime.Seconds.ToString();
        }
    }

    private string _remainingMinutes = "";

    public string RemainingMinutes
    {
        get => _remainingMinutes;
        private set
        {
            int.TryParse(value, out var min);
            _remainingMinutes = min < 10 ? $"0{min}" : $"{min}";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingMinutes)));
        }
    }

    private string _remainingSeconds = "";

    public string RemainingSeconds
    {
        get => _remainingSeconds;
        private set
        {
            int.TryParse(value, out var sec);
            _remainingSeconds = sec < 10 ? $"0{sec}" : $"{sec}";
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
        StartCountDown(TimeSpan.FromMinutes(WorkTime));
    }

    public void StartBreak()
    {
        _state = PomodoroState.Break;
        StartCountDown(TimeSpan.FromMinutes(BreakTime));
    }

    private void FinishCountDown()
    {
        PauseCountDown();
        SystemSounds.Hand.Play();

        if (_state == PomodoroState.Work)
        {
            var finishedWorkView = new FinishedWorkView();
            finishedWorkView.Show();
            finishedWorkView.Topmost = true;
        }
        else if (_state == PomodoroState.Break)
        {
            var finishedBreakView = new FinishedBreakView();
            finishedBreakView.Show();
            finishedBreakView.Topmost = true;
        }
    }

    public void InitView()
    {
        RemainingTime = TimeSpan.FromMinutes(WorkTime);
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