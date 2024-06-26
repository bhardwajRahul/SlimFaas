﻿using MemoryPack;
using SlimData.Commands;

namespace SlimData.Tests;

public class CommandsTests
{
    private protected static byte[] RandomBytes(int size)
    {
        var result = new byte[size];
        Random.Shared.NextBytes(result);
        return result;
    }

    [Fact]
    public static async Task InterpreterWithPersistentState()
    {
        byte[] bytes = RandomBytes(1000);
        using var wal = new SlimPersistentState(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
        var entry1 = wal.CreateLogEntry(new ListLeftPushCommand { Key  = "youhou" , Value  = bytes });
        await wal.AppendAsync(entry1);
        Assert.Empty(wal.SlimDataState.queues);
        await wal.CommitAsync(CancellationToken.None);
        Assert.Equal(bytes, wal.SlimDataState.queues["youhou"].First().ToArray());

        var bin = MemoryPackSerializer.Serialize(3);
        var final = MemoryPackSerializer.Deserialize<int>(bin);
        Assert.Equal(3, final);
    }


}
