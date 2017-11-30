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
            return await Get<T>(id, 0);
        }

        private async Task<T> Get<T>(Guid id, byte attempt)
        {
            try
            {
                return await base.Get<T>(id);
            }
            catch (DocumentClientException ex) when (ex.StatusCode == (HttpStatusCode)429)
            {
                if (!await this.IsRetriable(attempt))
                    throw;

                return await this.Get<T>(id, ++attempt);
            }
        }

        private async Task<bool> IsRetriable(byte attempt)
        {
            var aFutherAttemptShouldBeMade = attempt < this.Options.MaxRetries - 1;

            if (aFutherAttemptShouldBeMade)
            {
                await Task.Delay(50);
                return true;
            }

            return false;
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
