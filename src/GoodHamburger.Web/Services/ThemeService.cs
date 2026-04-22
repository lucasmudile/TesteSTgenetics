namespace GoodHamburger.Web.Services;

public class ThemeService
{
    public bool IsLight { get; private set; } = false;

    public event Action? OnThemeChanged;

    public void SetLight(bool isLight)
    {
        if (IsLight == isLight) return;
        IsLight = isLight;
        OnThemeChanged?.Invoke();
    }

    public void Toggle() => SetLight(!IsLight);

    public string ThemeClass => IsLight ? "light" : "";
}
