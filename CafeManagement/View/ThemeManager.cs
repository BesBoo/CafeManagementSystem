using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeManagement
{
    public static class ThemeManager
    {
        public static string CurrentTheme { get; private set; } = "Light"; 
        public static event Action<string> ThemeChanged;
        public static void SetTheme(string theme)
        {
            if (theme == "Light" || theme == "Dark")
            {
                CurrentTheme = theme;
                ThemeChanged?.Invoke(theme);
            }
        }
    }
}
