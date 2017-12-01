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
            var attemptResult = new RetriableActivity<T>();

            do
            {
                attemptResult = await this.Get<T>(id, attemptResult.AttemptsMade);
            } while (attemptResult.IsRetriable);

            if (attemptResult.WasSuccessful)
                return attemptResult.Result;

            throw attemptResult.Exception;
        }

        private async Task<RetriableActivity<T>> Get<T>(Guid id, int attempt)
        {
            try
            {
                // Make the attempt.
                var result = await base.Get<T>(id);
                return RetriableActivity<T>.Successful(result, attempt + 1);
            }
            catch (DocumentClientException ex) when (ex.StatusCode == (HttpStatusCode)429)
            {
                // Determine if we should retry based on this instances RetryOptions - the wait phase is baked into this method.
                if (!await this.IsRetriable(attempt))
                    return RetriableActivity<T>.Failure(ex, attempt + 1);

                // Recursively call this method.
                return RetriableActivity<T>.Retriable(ex, attempt + 1);
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