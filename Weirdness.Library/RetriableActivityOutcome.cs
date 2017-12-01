using System.ComponentModel;

namespace Weirdness.Library
{
    using System;

    public class RetriableActivityOutcome<T>
    {
        private enum RetriableActivityOutcomeClassification
        {
            NoAttemptMade,
            Successful,
            Failed,
            Retriable
        }

        public int AttemptsMade { get; }

        public T Result { get; }

        public Exception Exception { get; }

        private RetriableActivityOutcomeClassification Classification { get; }

        public bool IsRetriable => Classification == RetriableActivityOutcomeClassification.Retriable;

        public bool WasSuccessful => Classification == RetriableActivityOutcomeClassification.Successful;

        public RetriableActivityOutcome()
            : this(default(T), null, 0, RetriableActivityOutcomeClassification.NoAttemptMade)
        {
        }

        private RetriableActivityOutcome(T result, Exception exception, int attemptsMade, RetriableActivityOutcomeClassification classification)
        {
            Result = result;
            AttemptsMade = attemptsMade;
            Exception = exception;
            Classification = classification;
        }

        public static RetriableActivityOutcome<T> Successful(T result, int attemptsMade)
        {
            return new RetriableActivityOutcome<T>(result, null, attemptsMade, RetriableActivityOutcomeClassification.Successful);
        }

        public static RetriableActivityOutcome<T> Failure(Exception exception, int attemptsMade)
        {
            return new RetriableActivityOutcome<T>(default(T), exception, attemptsMade, RetriableActivityOutcomeClassification.Failed);
        }

        public static RetriableActivityOutcome<T> Retriable(Exception exception, int attemptsMade)
        {
            return new RetriableActivityOutcome<T>(default(T), exception, attemptsMade, RetriableActivityOutcomeClassification.Retriable);
        }
    }
}