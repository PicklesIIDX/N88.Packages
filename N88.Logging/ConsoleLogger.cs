namespace N88.Logging
{
    using System;
    using System.Collections.Generic;

    public class ConsoleLogger : ILogger
    {
        private readonly Dictionary<string, LoggingSettings> _categories = new();
        private readonly LoggingSettings _defaultSettings = new(string.Empty, string.Empty, string.Empty, string.Empty, true);
        
        public const string Black = "#000000";
        public const string DarkBlue = "#000080";
        public const string DarkGreen = "#008000";
        public const string DarkCyan = "#008080";
        public const string DarkRed = "#800000";
        public const string DarkMagenta = "#800080";
        public const string DarkYellow = "#808000";
        public const string Gray = "#c0c0c0";
        public const string DarkGray = "#808080";
        public const string Blue = "#0000ff";
        public const string Green = "#00ff00";
        public const string Cyan = "#00ffff";
        public const string Red = "#ff0000";
        public const string Magenta = "#ff00ff";
        public const string Yellow = "#ffff00";
        public const string White = "#ffffff";

        public ConsoleLogger(params LoggingSettings[] categories)
        {
            foreach (var category in categories)
            {
                _categories.Add(category.Category, category);
            }
        }

        public void Log(string text, string? category = null)
        {
            var settings = _defaultSettings; 
            if (!string.IsNullOrEmpty(category) && !_categories.TryGetValue(category, out settings)) return;
            if(!settings.Enabled){return;}
            Console.ForegroundColor = HexToConsoleColor(settings.Color);
            var timestamp = string.Empty;
            if (!string.IsNullOrEmpty(settings.TimeFormat))
            {
                timestamp = $"[{DateTime.Now.ToString(settings.TimeFormat)}]: ";
            }
            
            Console.Write($"{timestamp}{settings.DisplayName}{text}");
        }

        private ConsoleColor HexToConsoleColor(string settingsColor)
        {
            return settingsColor switch
            {
                Black => ConsoleColor.Black,
                DarkBlue => ConsoleColor.DarkBlue,
                DarkGreen => ConsoleColor.DarkGreen,
                DarkCyan => ConsoleColor.DarkCyan,
                DarkRed => ConsoleColor.DarkRed,
                DarkMagenta => ConsoleColor.DarkMagenta,
                DarkYellow => ConsoleColor.DarkYellow,
                Gray => ConsoleColor.Gray,
                DarkGray => ConsoleColor.DarkGray,
                Blue => ConsoleColor.Blue,
                Green => ConsoleColor.Green, 
                Cyan => ConsoleColor.Cyan,
                Red => ConsoleColor.Red,
                Magenta => ConsoleColor.Magenta,
                Yellow => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };
        }
    }
}