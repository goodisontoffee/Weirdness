namespace Weirdness.Library
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;

    public class RetriableGetStuff : GetStuffBase
    {
        private RetryOptions Options { get; }

        public RetriableGetStuff(
            IDocumentRepository repository,
            RetryOptions options)
            : base(repository)
        {
            Options = options;
        }

        public override async Task<T> Get<T>(Guid id)
        {
            // Start our first attempt.
            return await this.Get<T>(id, 0);
        }

        private async Task<T> Get<T>(Guid id, int attempt)
        {
            try
            {
                // Make the attempt.
                return await base.Get<T>(id);
            }
            catch (DocumentClientException ex) when (ex.StatusCode == (HttpStatusCode)429)
            {
                // Determine if we should retry based on this instances RetryOptions - the wait phase is baked into this method.
                if (!await this.IsRetriable(attempt))
                    throw;

                // Recursively call this method.
                return await this.Get<T>(id, attempt + 1);
            }
        }

        private async Task<bool> IsRetriable(int attempt)
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
}