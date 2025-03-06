﻿using SlimData;

namespace SlimFaas.Database;

public class JobQueue(IDatabaseService databaseService) : IJobQueue
{
    private const string KeyPrefix = "Job:";

    public async Task EnqueueAsync(string key, byte[] data)
    {
        RetryInformation retryInformation = new(new List<int>(), 300, new List<int>());
        await databaseService.ListLeftPushAsync($"{KeyPrefix}{key}", data, retryInformation);
    }

    public async Task<IList<QueueData>?> DequeueAsync(string key, int count = 1)
    {
        var data = await databaseService.ListRightPopAsync($"{KeyPrefix}{key}", count);
        return data;
    }

    public async Task ListCallbackAsync(string key, ListQueueItemStatus queueItemStatus) => await databaseService.ListCallbackAsync($"{KeyPrefix}{key}", queueItemStatus);

    public async Task<long> CountElementAsync(string key, IList<CountType> countTypes, int maximum = int.MaxValue) => await databaseService.ListCountElementAsync($"{KeyPrefix}{key}", countTypes, maximum);

}
