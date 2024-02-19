using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ClockReadabilityTest;

public record IndicatorValueItem(BaseValue Value, int Key, bool KeyVisible);

public sealed class ViewModel : INotifyPropertyChanged, IDisposable
{
    private CancellationTokenSource? _cts;
    private readonly Stopwatch _stopwatch = new();
    private readonly ManualResetEventSlim _reactionEvent = new();
    private readonly ManualResetEventSlim _continueEvent = new();
    private readonly ManualResetEventSlim _nextEvent = new();
    private bool _isTime = true;
    private bool _isRunning;
    private bool _isAsked;
    private bool _isCounting;
    private bool _isAnswered;
    private string? _text1;
    private int _countdown;
    private int? _resultAnalog;
    private IReadOnlyList<IndicatorValueItem>? _values;
    private int _numValues = 1;
    private int _numTestPairs = 10;
    private int _countdownSeconds = 3;
    private bool _round;
    private int? _resultDigital;
    private string? _text2;
    private double _zoom = 1;

    public bool IsTime
    {
        get => _isTime;
        set => SetField(ref _isTime, value);
    }

    public bool IsRunning
    {
        get => _isRunning;
        set => SetField(ref _isRunning, value);
    }

    public bool IsCounting
    {
        get => _isCounting;
        set => SetField(ref _isCounting, value);
    }

    public bool IsAsked
    {
        get => _isAsked;
        set => SetField(ref _isAsked, value);
    }

    public bool IsAnswered
    {
        get => _isAnswered;
        set => SetField(ref _isAnswered, value);
    }

    public string? Text1
    {
        get => _text1;
        set => SetField(ref _text1, value);
    }

    public string? Text2
    {
        get => _text2;
        set => SetField(ref _text2, value);
    }

    public int Countdown
    {
        get => _countdown;
        set => SetField(ref _countdown, value);
    }

    public int? ResultAnalog
    {
        get => _resultAnalog;
        set => SetField(ref _resultAnalog, value);
    }

    public int? ResultDigital
    {
        get => _resultDigital;
        set => SetField(ref _resultDigital, value);
    }


    public int NumValues
    {
        get => _numValues;
        set
        {
            SetField(ref _numValues, value);
            OnPropertyChanged(nameof(SingleMode));
        }
    }

    public int NumTestPairs
    {
        get => _numTestPairs;
        set => SetField(ref _numTestPairs, value);
    }

    public int CountdownSeconds
    {
        get => _countdownSeconds;
        set => SetField(ref _countdownSeconds, value);
    }

    public bool Round
    {
        get => _round;
        set => SetField(ref _round, value);
    }

    public double Zoom
    {
        get => _zoom;
        set => SetField(ref _zoom, value);
    }

    public bool SingleMode => NumValues == 1;

    public IReadOnlyList<IndicatorValueItem>? Values
    {
        get => _values;
        set => SetField(ref _values, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) 
            return;
        field = value;
        OnPropertyChanged(propertyName);
    }

    public void Dispose()
    {
        _cts?.Dispose();
        _reactionEvent.Dispose();
        _continueEvent.Dispose();
        _nextEvent.Dispose();
    }

    public async void Start()
    {
        try
        {
            IsRunning = true;
            ResultAnalog = null;
            ResultDigital = null;
            
            var random = new Random(DateTime.Now.Millisecond);

            var testPlan = new BaseValue[NumTestPairs * 2, NumValues];
            
            int GetMinute() => RoundValue(random.Next(0, 59));
            int GetSpeed() => RoundValue(random.Next(0, 270));
            int GetHour() => random.Next(0, 11);
            
            for (var i = 0; i < testPlan.GetLength(0); i++)
            {
                var expectedIndex = SingleMode ? i >= NumTestPairs ? 1 :0 : random.Next(NumValues);
                for (var j = 0; j < NumValues; j++)
                {
                    var isDigital = i % 2 == 1;
                    var isExpected = j == expectedIndex;
                    
                    if (IsTime)
                    {
                        if (isDigital)
                        {
                            if (isExpected)
                            {
                                testPlan[i, j] = new DigitalTime(GetHour(), GetMinute());
                            }
                            else
                            {
                                testPlan[i, j] = new IncorrectDigitalTime(GetHour(), GetMinute(), GetHour(), GetMinute());
                            }
                        }
                        else
                        {
                            if (isExpected)
                            {
                                testPlan[i, j] = new AnalogTime(GetHour(), GetMinute());
                            }
                            else
                            {
                                testPlan[i, j] = new IncorrectAnalogTime(GetHour(), GetMinute(), GetHour(), GetMinute());
                            }
                        }
                    }
                    else
                    {
                        if (isDigital)
                        {
                            if (isExpected)
                            {
                                testPlan[i, j] = new DigitalSpeed(GetSpeed());
                            }
                            else
                            {
                                testPlan[i, j] = new IncorrectDigitalSpeed(GetSpeed(), GetSpeed());
                            }
                        }
                        else
                        {
                            if (isExpected)
                            {
                                testPlan[i, j] = new AnalogSpeed(GetSpeed());
                            }
                            else
                            {
                                testPlan[i, j] = new IncorrectAnalogSpeed(GetSpeed(), GetSpeed());
                            }
                        }
                    }
                }
            }

            for (var r = 0; r < 10; r++)
            {
                var n = testPlan.GetLength(0);
                while (n > 1)
                {
                    n--;
                    var k = random.Next(n + 1);
                    for (var i = 0; i < testPlan.GetLength(1); i++)
                        (testPlan[n, i], testPlan[k, i]) = (testPlan[k, i], testPlan[n, i]);
                }
            }

            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            for (var i = 0; i < testPlan.GetLength(0); i++)
            {
                var collection = new BaseValue[testPlan.GetLength(1)];
                for (var j = 0; j < collection.Length; j++)
                    collection[j] = testPlan[i, j];

                await Task.Yield();

                Values = null;
                var current = collection.SingleOrDefault(y => y.IsExpected) ?? collection.Single();
                Text1 = current.Text1;
                Text2 = current.Text2;
                IsAnswered = false;
                IsAsked = true;
                IsCounting = false;
                IsAnswered = false;

                _continueEvent.Reset();
                await Task.Factory.StartNew(() => _continueEvent.Wait(_cts.Token), _cts.Token);

                for (var t = CountdownSeconds; t > 0; t--)
                {
                    await Task.Yield();
                    IsAsked = false;
                    IsCounting = true;
                    Countdown = t;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                await Task.Yield();

                Countdown = 0;
                Values = collection.Select((x, index) => new IndicatorValueItem(x, index + 1, !SingleMode)).ToArray();
                _stopwatch.Restart();

                _reactionEvent.Reset();
                await Task.Factory.StartNew(() => _reactionEvent.Wait(_cts.Token), _cts.Token);

                await Task.Yield();
                
                IsCounting = false;
                IsAnswered = true;
                
                _nextEvent.Reset();
                await Task.Factory.StartNew(() => _nextEvent.Wait(_cts.Token), _cts.Token);
            }

            await Task.Yield();

            IsRunning = false;
            
            var timesAnalog = new List<TimeSpan>();
            var timesDigital = new List<TimeSpan>();
            for (var i = 0; i < testPlan.GetLength(0); i++)
            {
                for (var j = 0; j < testPlan.GetLength(1); j++)
                {
                    if (testPlan[i, j].IsValueSelectedCorrect)
                    {
                        (testPlan[i, j].IsAnalog ? timesAnalog : timesDigital).Add(testPlan[i, j].Time);
                    }
                }
            }

            ResultAnalog = GetMedian(timesAnalog);
            ResultDigital = GetMedian(timesDigital);
        }
        catch (TaskCanceledException)
        {
            Stop();
        }
        catch (OperationCanceledException)
        {
            Stop();
        }
    }

    public static int GetMedian(List<TimeSpan> source)
    {
        source.Sort();
 
        var mid = source.Count / 2;
 
        if (source.Count % 2 != 0)
            return (int)source[mid].TotalMilliseconds;
 
        return (int)((source[mid].TotalMilliseconds + source[mid - 1].TotalMilliseconds) / 2);
    }
    
    private int RoundValue(int value) => Round ? (int)(Math.Round(value / 5.0) * 5) : value;

    public void Stop()
    {
        IsRunning = false;
        IsCounting = false;
        IsAnswered = false;
        IsAsked = false;
        if (_cts?.IsCancellationRequested == false)
            _cts?.Cancel();
    }

    public void Ready()
    {
        if (IsAsked)
            _continueEvent.Set();
        if (IsAnswered)
            _nextEvent.Set();
    }

    public void Select(int index)
    {
        if (Values == null || index < 0 || (!SingleMode && index >= Values.Count) || (SingleMode && index > 1))
            return;
        _stopwatch.Stop();

        var chooseExpected = !SingleMode || index == 0;
        var value = SingleMode ? Values[0] : Values[index];
            
        value.Value.Click(_stopwatch.Elapsed, chooseExpected);
        if (value.Value.IsValueSelectedCorrect)
            _reactionEvent.Set();
    }
}