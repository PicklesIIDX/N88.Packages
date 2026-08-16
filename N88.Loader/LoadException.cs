namespace N88.Loader
{
    using System;

    public class LoadException : Exception
    {
        public LoadException(string message): base(message) { }
    }
}