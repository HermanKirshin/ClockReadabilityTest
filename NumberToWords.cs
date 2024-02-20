using System;
using System.Globalization;

namespace ClockReadabilityTest;

internal static class NumberToWords
{
    public static string Convert(int value)
    {
        if (Equals(CultureInfo.CurrentUICulture, CultureInfo.GetCultureInfo("EN-us")))
            return ConvertEn(value);

        if (Equals(CultureInfo.CurrentUICulture, CultureInfo.GetCultureInfo("RU-ru")))
            return ConvertRu(value);

        throw new NotSupportedException();
    }

    private static string ConvertEn(int value)
    {
        var result = "";

        if (value == 0)
            return "zero";
        
        if (value / 100 > 0)
        {
            result += Convert(value / 100) + " hundred ";
            value %= 100;
        }

        if (value <= 0) 
            return result;
        
        if (result != "")
            result += "and ";

        var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

        if (value < 20)
            result += unitsMap[value];
        else
        {
            result += tensMap[value / 10];
            if ((value % 10) > 0)
                result += "-" + unitsMap[value % 10];
        }

        return result;
    }

    private static string ConvertRu(int value)
    {
        var result = "";

        if (value == 0)
            return "ноль";
        
        if (value / 100 > 0)
        {
            result += (value / 100) switch
            {
                1 => " сто ",
                2 => " двести ",
                _ => throw new ArgumentOutOfRangeException()
            };
            value %= 100;
        }

        if (value <= 0) 
            return result;
        
        var unitsMap = new[] { "ноль", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять", "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" };
        var tensMap = new[] { "ноль", "десять", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };

        if (value < 20)
            result += unitsMap[value];
        else
        {
            result += tensMap[value / 10];
            if ((value % 10) > 0)
                result += " " + unitsMap[value % 10];
        }

        return result;
    }
}