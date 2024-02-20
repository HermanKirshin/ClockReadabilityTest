using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClockReadabilityTest;

public class IncorrectAnalogTime(int hour, int minute, int incorrectHour, int incorrectMinute)
    : AnalogTime(hour, minute)
{
    public override bool IsExpected => false;

    public override string Text1 => NumberToWords.Convert(incorrectHour);
    public override string Text2 => NumberToWords.Convert(incorrectMinute);
}

public class IncorrectDigitalTime(int hour, int minute, int incorrectHour, int incorrectMinute)
    : DigitalTime(hour, minute)
{
    public override bool IsExpected => false;

    public override string Text1 => NumberToWords.Convert(incorrectHour);
    public override string Text2 => NumberToWords.Convert(incorrectMinute);
}

public class IncorrectAnalogSpeed(int value, int incorrectValue) : AnalogSpeed(value)
{
    public override bool IsExpected => false;

    public override string Text1 => NumberToWords.Convert(incorrectValue);
    public override string Text2 => string.Empty;
}

public class IncorrectDigitalSpeed(int value, int incorrectValue) : DigitalSpeed(value)
{
    public override bool IsExpected => false;

    public override string Text1 => NumberToWords.Convert(incorrectValue);
    public override string Text2 => string.Empty;
}


public class AnalogTime(int hour, int minute) : BaseValue
{
    public override bool IsAnalog => true;
    
    public int HourAngle
    {
        get;
    } = hour * 30;

    public int MinuteAngle
    {
        get;
    } = minute * 6;

    public override string Text1 => NumberToWords.Convert(hour);
    public override string Text2 => NumberToWords.Convert(minute);
}

public class DigitalTime(int hour, int minute) : BaseValue
{
    public override bool IsAnalog => false;
    
    public string TimeString
    {
        get;
    } = $"{hour}:{minute:00}";

    public override string Text1 => NumberToWords.Convert(hour);
    public override string Text2 => NumberToWords.Convert(minute);
}

public class AnalogSpeed(int value) : BaseValue
{
    private readonly int _value = value;

    public override bool IsAnalog => true;
    
    public int Angle
    {
        get;
    } = value;

    public override string Text1 => NumberToWords.Convert(_value);
    public override string Text2 => string.Empty;
}

public class DigitalSpeed(int value) : BaseValue
{
    public override bool IsAnalog => false;

    public int Value
    {
        get;
    } = value;

    public override string Text1 => NumberToWords.Convert(Value);
    public override string Text2 => string.Empty;
}

public abstract class BaseValue : INotifyPropertyChanged
{
    private bool _isValueSelectedCorrect;
    private bool _isValueSelectedIncorrect;

    public virtual bool IsExpected => true;
    public abstract bool IsAnalog { get; }

    public abstract string Text1 { get; }
    public abstract string Text2 { get; }
    
    public TimeSpan Time
    {
        get;
        private set;
    }

    public void Click(TimeSpan time, bool chooseExpected)
    {
        if ((IsExpected && chooseExpected) || (!IsExpected && !chooseExpected))
        {
            IsValueSelectedIncorrect = false;
            IsValueSelectedCorrect = true;
        }
        else
            IsValueSelectedIncorrect = true;

        if (Time == default)
            Time = time;
    }

    public bool IsValueSelectedCorrect
    {
        get => _isValueSelectedCorrect;
        private set => SetField(ref _isValueSelectedCorrect, value);
    }

    public bool IsValueSelectedIncorrect
    {
        get => _isValueSelectedIncorrect;
        private set => SetField(ref _isValueSelectedIncorrect, value);
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
}