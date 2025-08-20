using MudBlazor;
using System.ComponentModel;

namespace ReservaPeriferico.Web.Services;

public class ThemeService : INotifyPropertyChanged
{
    private bool _isDarkMode = false;
    private MudTheme _currentTheme;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
                _currentTheme = _isDarkMode ? DarkTheme : LightTheme;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDarkMode)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTheme)));
            }
        }
    }

    public MudTheme CurrentTheme
    {
        get => _currentTheme;
        private set
        {
            _currentTheme = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTheme)));
        }
    }

    public ThemeService()
    {
        // Inicializar com tema light por padrão
        _currentTheme = LightTheme;
    }

    public void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
    }

    public void SetTheme(bool isDarkMode)
    {
        IsDarkMode = isDarkMode;
    }

    private static readonly MudTheme LightTheme = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#002233",
            PrimaryDarken = "#012e45",
            Secondary = "#a44dff",
            Info = "#00dbff",
            Background = "#ffffff",
            Surface = "#ffffff",
            AppbarBackground = "#002233",
            DrawerBackground = "#ffffff",
            DrawerText = "#424242",
            TextPrimary = "#424242",
            TextSecondary = "#666666"
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#00dbff",
            PrimaryDarken = "#012e45",
            Secondary = "#a44dff",
            Info = "#00dbff",
            Background = "#1a1a1a",
            Surface = "#2d2d2d",
            AppbarBackground = "#1a1a1a",
            DrawerBackground = "#2d2d2d",
            DrawerText = "#ffffff"
        }
    };

    private static readonly MudTheme DarkTheme = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#00dbff",
            PrimaryDarken = "#012e45",
            Secondary = "#a44dff",
            Info = "#00dbff",
            Background = "#1a1a1a",
            Surface = "#2d2d2d",
            AppbarBackground = "#1a1a1a",
            DrawerBackground = "#2d2d2d",
            DrawerText = "#ffffff",
            TextPrimary = "#ffffff",
            TextSecondary = "#b0b0b0"
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#00dbff",
            PrimaryDarken = "#012e45",
            Secondary = "#a44dff",
            Info = "#00dbff",
            Background = "#1a1a1a",
            Surface = "#2d2d2d",
            AppbarBackground = "#1a1a1a",
            DrawerBackground = "#2d2d2d",
            DrawerText = "#ffffff",
            TextPrimary = "#ffffff",
            TextSecondary = "#b0b0b0"
        }
    };
}
