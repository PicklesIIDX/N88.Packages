namespace N88.Logging
{
    using System;

    public struct LoggingSettings
    {
        public readonly string Category;
        public readonly string DisplayName;
        public readonly string Color;
        public readonly string TimeFormat;
        public readonly bool Enabled;
        
        public LoggingSettings(
            string category, 
            string displayName, 
            string color,
            string timeFormat,
            bool enabled = true)
        {
            Category = category;
            DisplayName = displayName;
            Color = color;
            TimeFormat = timeFormat;
            Enabled = enabled;
        }
    }
}