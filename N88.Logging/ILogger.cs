namespace N88.Logging
{
    public interface ILogger
    {
        public void Log(string text, string? category = null);
    }
}