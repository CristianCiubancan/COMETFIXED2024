// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - PacketProcessor.cs
// Description:
// 
// Creator: FELIPEVIEIRAVENDRAMI [FELIPE VIEIRA VENDRAMINI]
// 
// Developed by:
// Felipe Vieira Vendramini <felipevendramini@live.com>
// 
// Programming today is a race between software engineers striving to build bigger and better
// idiot-proof programs, and the Universe trying to produce bigger and better idiots.
// So far, the Universe is winning.
// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#region References

using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Comet.Network.Sockets;
using Comet.Shared;
using Microsoft.Extensions.Hosting;

#endregion

namespace Comet.Network.Packets
{
    /// <summary>
    ///     Packet processor for handling packets in background tasks using unbounded
    ///     channel. Allows for multiple writers, such as each remote client's accepted socket
    ///     receive loop, to write to an assigned channel. Each reader has an associated
    ///     channel to guarantee client packet processing order.
    /// </summary>
    /// <typeparam name="TClient">Type of client being processed with the packet</typeparam>
public class PacketProcessor<TClient> where TClient : TcpServerActor
{

    private Channel<Message> m_channel;
    private CancellationTokenSource m_cts;
    private Task m_processTask;
    private Func<TClient, byte[], Task> Process;
    public PacketProcessor(Func<TClient, byte[], Task> process)
    {
        m_channel = Channel.CreateUnbounded<Message>();
        m_cts = new CancellationTokenSource();
        Process = process;
    }

    public void Queue(TClient actor, byte[] packet) 
    {
        m_channel.Writer.TryWrite(new Message { Actor = actor, Packet = packet });
    }

    protected Task ExecuteAsync(CancellationToken token)
    {
        m_processTask = ProcessMessages(m_cts.Token);
        return m_processTask;
    }

    private async Task ProcessMessages(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var msg = await m_channel.Reader.ReadAsync(token);
            if(msg != null)
            {
                await Process(msg.Actor, msg.Packet); 
            }
        }
    }
        protected class Message
        {
            public TClient Actor;
            public byte[] Packet;
        }
    public Task StopAsync(CancellationToken token)
    {
        m_cts.Cancel();
        return Task.CompletedTask;
        // return base.StopAsync(token);
    }

}




        // Fields and Properties
        // protected readonly Task[] BackgroundTasks;
        // protected readonly Channel<Message>[] Channels;
        // protected readonly Partition[] Partitions;
        // protected readonly Func<TClient, byte[], Task> Process;
        // protected CancellationToken CancelReads;
        // protected CancellationToken CancelWrites;

        /// <summary>
        ///     Instantiates a new instance of <see cref="PacketProcessor" /> using a default
        ///     amount of worker tasks to initialize. Tasks will not be started.
        /// </summary>
        /// <param name="process">Processing task for channel messages</param>
        /// <param name="count">Number of threads to be created</param>
        // public PacketProcessor(
        //     Func<TClient, byte[], Task> process,
        //     int count = 0)
        // {

        //     // Initialize the channels and tasks as parallel arrays
        //     count = Math.Max(1, count == 0 ? Environment.ProcessorCount < 6 && Environment.ProcessorCount >= 2 ? Environment.ProcessorCount : Environment.ProcessorCount / 2 : count);
        //     BackgroundTasks = new Task[count];
        //     CancelReads = new CancellationToken();
        //     CancelWrites = new CancellationToken();
        //     Channels = new Channel<Message>[count];
        //     Partitions = new Partition[count];
        //     Process = process;
        // }

        // /// <summary>
        // ///     Triggered when the application host is ready to execute background tasks for
        // ///     dequeuing and processing work from unbounded channels. Work is queued by a
        // ///     connected and assigned client.
        // /// </summary>
        // protected override Task ExecuteAsync(CancellationToken stoppingToken)
        // {
        //     for (int i = 0; i < BackgroundTasks.Length; i++)
        //     {
        //         Partitions[i] = new Partition { ID = (uint)i };
        //         Channels[i] = Channel.CreateUnbounded<Message>();
        //         BackgroundTasks[i] = DequeueAsync(Channels[i]);
        //     }

        //     return Task.WhenAll(BackgroundTasks);
        // }

        // /// <summary>
        // ///     Queues work by writing to a message channel. Work is queued by a connected
        // ///     client, and dequeued by the server's packet processing worker tasks. Each
        // ///     work item contains a single packet to be processed.
        // /// </summary>
        // /// <param name="actor">Actor requesting packet processing</param>
        // /// <param name="packet">Packet bytes to be processed</param>
        // public void Queue(TClient actor, byte[] packet)
        // {
        //     if (!CancelWrites.IsCancellationRequested)
        //         Channels[actor.Partition].Writer.TryWrite(new Message
        //         {
        //             Actor = actor,
        //             Packet = packet
        //         });
        // }

        // /// <summary>
        // ///     Dequeues work in a loop. For as long as the thread is running and work is
        // ///     available, work will be dequeued and processed. After dequeuing a message,
        // ///     the packet processor's <see cref="Process" /> action will be called.
        // /// </summary>
        // /// <param name="channel">Channel to read messages from</param>
        // protected async Task DequeueAsync(Channel<Message> channel)
        // {
        //     while (!CancelReads.IsCancellationRequested)
        //     {
        //         var msg = await channel.Reader.ReadAsync(CancelReads);
        //         if (msg != null)
        //         {
        //             try
        //             {
        //                 await Process(msg.Actor, msg.Packet).ConfigureAwait(false);
        //             }
        //             catch (Exception ex)
        //             {
        //                 _ = Log.WriteLogAsync(LogLevel.Socket, ex.ToString()).ConfigureAwait(false);
        //             }
        //         }
        //     }
        // }

        // /// <summary>
        // ///     Triggered when the application host is stopping the background task with a
        // ///     graceful shutdown. Requests that writes into the channel stop, and then reads
        // ///     from the channel stop.
        // /// </summary>
        // public new async Task StopAsync(CancellationToken cancellationToken)
        // {
        //     CancelWrites = new CancellationToken(true);
        //     foreach (var channel in Channels)
        //         await channel.Reader.Completion;
        //     CancelReads = new CancellationToken(true);
        //     await base.StopAsync(cancellationToken);
        // }

        // /// <summary>
        // ///     Selects a partition for the client actor based on partition weight. The
        // ///     partition with the least popluation will be chosen first. After selecting a
        // ///     partition, that partition's weight will be increased by one.
        // /// </summary>
        // public uint SelectPartition()
        // {
        //     uint partition = Partitions.Aggregate((aggr, next) =>
        //         next.Weight.CompareTo(aggr.Weight) < 0 ? next : aggr).ID;
        //     Interlocked.Increment(ref Partitions[partition].Weight);
        //     return partition;
        // }

        // /// <summary>
        // ///     Deslects a partition after the client actor disconnects.
        // /// </summary>
        // /// <param name="partition">The partition id to reduce the weight of</param>
        // public void DeselectPartition(uint partition)
        // {
        //     Interlocked.Decrement(ref Partitions[partition].Weight);
        // }

        // /// <summary>
        // ///     Defines a message for the <see cref="PacketProcessor" />'s unbounded channel
        // ///     for queuing packets and actors requesting work. Each message defines a single
        // ///     unit of work - a single packet for processing.
        // /// </summary>
        // protected class Message
        // {
        //     public TClient Actor;
        //     public byte[] Packet;
        // }

        // /// <summary>
        // ///     Defines a partition for the <see cref="PacketProcessor" />. This allows the
        // ///     background service to track partition weight and assign clients to less
        // ///     populated partitions.
        // /// </summary>
        // protected class Partition
        // {
        //     public uint ID;
        //     public int Weight;
        // }
    // }
}