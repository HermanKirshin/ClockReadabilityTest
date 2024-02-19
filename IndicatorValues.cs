using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClockReadabilityTest;

public class IncorrectAnalogTime : AnalogTime
{
    private readonly int _incorrectHour;
    private readonly int _incorrectMinute;
    
    public override bool IsExpected => false;

    public IncorrectAnalogTime(int hour, int minute, int incorrectHour, int incorrectMinute) : base(hour, minute)
    {
        _incorrectHour = incorrectHour;
        _incorrectMinute = incorrectMinute;
    }

    public override string Text1 => NumberToWords.Convert(_incorrectHour);
    public override string Text2 => NumberToWords.Convert(_incorrectMinute);
}

public class IncorrectDigitalTime : DigitalTime
{
    private readonly int _incorrectHour;
    private readonly int _incorrectMinute;
    
    public override bool IsExpected => false;
    
    public IncorrectDigitalTime(int hour, int minute, int incorrectHour, int incorrectMinute) : base(hour, minute)
    {
        _incorrectHour = incorrectHour;
        _incorrectMinute = incorrectMinute;
    }

    public override string Text1 => NumberToWords.Convert(_incorrectHour);
    public override string Text2 => NumberToWords.Convert(_incorrectMinute);
}

public class IncorrectAnalogSpeed : AnalogSpeed
{
    private readonly int _incorrectValue;
    
    public override bool IsExpected => false;
    
    public IncorrectAnalogSpeed(int value, int incorrectValue) : base(value)
    {
        _incorrectValue = incorrectValue;
    }

    public override string Text1 => NumberToWords.Convert(_incorrectValue);
    public override string Text2 => string.Empty;
}

public class IncorrectDigitalSpeed : DigitalSpeed
{
    private readonly int _incorrectValue;
    
    public override bool IsExpected => false;
    
    public IncorrectDigitalSpeed(int value, int incorrectValue) : base(value)
    {
        _incorrectValue = incorrectValue;
    }

    public override string Text1 => NumberToWords.Convert(_incorrectValue);
    public override string Text2 => string.Empty;
}


public class AnalogTime : BaseValue
{
    private readonly int _hour;
    private readonly int _minute;
    
    public override bool IsAnalog => true;
    
    public int HourAngle
    {
        get;
    }
    
    public int MinuteAngle
    {
        get;
    }

    public AnalogTime(int hour, int minute)
    {
        _hour = hour;
        _minute = minute;
        HourAngle = hour * 30;
        MinuteAngle = minute * 6;
    }

    public override string Text1 => NumberToWords.Convert(_hour);
    public override string Text2 => NumberToWords.Convert(_minute);
}

public class DigitalTime : BaseValue
{
    private readonly int _hour;
    private readonly int _minute;
    
    public override bool IsAnalog => false;
    
    public string TimeString
    {
        get;
    }

    public DigitalTime(int hour, int minute)
    {
        _hour = hour;
        _minute = minute;
        TimeString = $"{hour}:{minute:00}";
    }

    public override string Text1 => NumberToWords.Convert(_hour);
    public override string Text2 => NumberToWords.Convert(_minute);
}

public class AnalogSpeed : BaseValue
{
    private readonly int _value;

    public override bool IsAnalog => true;
    
    public int Angle
    {
        get;
    }

    public AnalogSpeed(int value)
    {
        Angle = value;
        _value = value;
    }

    public override string Text1 => NumberToWords.Convert(_value);
    public override string Text2 => string.Empty;
}

public class DigitalSpeed : BaseValue
{
    public override bool IsAnalog => false;

    public int Value
    {
        get;
    }

    public DigitalSpeed(int value)
    {
        Value = value;
    }

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