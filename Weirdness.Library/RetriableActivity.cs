using System.ComponentModel;

namespace Weirdness.Library
{
    using System;

    public class RetriableActivity<T>
    {
        private enum RetriableActivityAttemptClassification
        {
            NoAttemptMade,
            Successful,
            Failed,
            Retriable
        }

        public int AttemptsMade { get; }

        public T Result { get; }

        public Exception Exception { get; }

        private RetriableActivityAttemptClassification Classification { get; }

        public bool IsRetriable => Classification == RetriableActivityAttemptClassification.Retriable;

        public bool WasSuccessful => Classification == RetriableActivityAttemptClassification.Successful;

        public RetriableActivity()
            : this(default(T), null, 0, RetriableActivityAttemptClassification.NoAttemptMade)
        {
        }

        private RetriableActivity(T result, Exception exception, int attemptsMade, RetriableActivityAttemptClassification classification)
        {
            Result = result;
            AttemptsMade = attemptsMade;
            Exception = exception;
            Classification = classification;
        }

        public static RetriableActivity<T> Successful(T result, int attemptsMade)
        {
            return new RetriableActivity<T>(result, null, attemptsMade, RetriableActivityAttemptClassification.Successful);
        }

        public static RetriableActivity<T> Failure(Exception exception, int attemptsMade)
        {
            return new RetriableActivity<T>(default(T), exception, attemptsMade, RetriableActivityAttemptClassification.Failed);
        }

        public static RetriableActivity<T> Retriable(Exception exception, int attemptsMade)
        {
            return new RetriableActivity<T>(default(T), exception, attemptsMade, RetriableActivityAttemptClassification.Retriable);
        }
    }
}