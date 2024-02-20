using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace ClockReadabilityTest;

public partial class MainWindow : Window
{
    public static readonly StyledProperty<ViewModel> DataProperty = AvaloniaProperty.Register<MainWindow, ViewModel>(nameof(Data));
    
    public MainWindow()
    {
        Set();
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("EN-us");
        InitializeComponent();
    }

    public ViewModel Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }
    
    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs _)
    {
        CultureInfo.CurrentUICulture = ((ComboBox)sender!).SelectedIndex switch
        {
            0 => CultureInfo.GetCultureInfo("EN-us"),
            1 => CultureInfo.GetCultureInfo("RU-ru"),
            _ => CultureInfo.CurrentUICulture
        };
        Set();
    }

    private void Set()
    {
        Data = null!;
        Data = new ViewModel();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Space)
            Data.Ready();
        
        Data.Select(e.Key switch
        {
            Key.NumPad1 => 0,
            Key.D1 => 0,
            Key.NumPad2 => 1,
            Key.D2 => 1,
            Key.NumPad3 => 2,
            Key.D3 => 2,
            Key.NumPad4 => 3,
            Key.D4 => 3,
            Key.NumPad5 => 4,
            Key.D5 => 4,
            Key.NumPad6 => 5,
            Key.D6 => 5,
            Key.NumPad7 => 6,
            Key.D7 => 6,
            Key.NumPad8 => 7,
            Key.D8 => 7,
            Key.NumPad9 => 8,
            Key.D9 => 8,
            _ => -1
        });
    }
}