using System;

namespace CommonLibraries.BackgroundWorker.Abstractions
{
    public interface IListBackgroundMetrics
    {
        int MaxDegreeOfParallelism { get; }

        long? LastIterationListItemsCount { get; }

        DateTime? LastIterationListItemExecuteDateTime { get; }

        long LastIterationListItemsSuccessProcessedCount { get; }

        long LastIterationListItemsProcessedCount { get; }

        long LastIterationListItemsAvgDurationInMilliseconds { get; }

        long LastIterationListItemsLastDurationInMilliseconds { get; }
    }
}
