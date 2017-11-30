using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace Weirdness.Library
{
    public class GetStuffBase
    {
        public virtual async Task<T> Get<T>(Guid id)
        {
            return await Task.FromResult(default(T));
        }
    }

    public class RetriableGetStuff : GetStuffBase
    {
        private RetryOptions Options { get; }

        public RetriableGetStuff(RetryOptions options)
        {
            Options = options;
        }

        public override async Task<T> Get<T>(Guid id)
        {
            for (byte attempt = 1; attempt <= this.Options.MaxRetries; attempt++)
            {
                try
                {
                    return await base.Get<T>(id);
                }
                catch (DocumentClientException ex) when (ex.StatusCode == (HttpStatusCode)429)
                {
                    if (attempt == this.Options.MaxRetries)
                        throw;

                    await this.IncreaseOrWait(attempt);
                }
            }
        }

        private async Task IncreaseOrWait(byte attempt)
        {
            if (attempt < this.Options.MaxRetries)
                await Task.Delay(50);
        }
    }

    public struct RetryOptions
    {
        public byte MaxRetries { get; }

        public RetryOptions(byte maxRetries)
        {
            MaxRetries = maxRetries;
        }
    }
}
