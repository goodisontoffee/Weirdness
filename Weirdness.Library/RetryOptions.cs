namespace Weirdness.Library
{
    public struct RetryOptions
    {
        public int MaxRetries { get; }

        public RetryOptions(int maxRetries)
        {
            MaxRetries = maxRetries;
        }
    }
}
