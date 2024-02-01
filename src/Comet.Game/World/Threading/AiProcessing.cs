// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Ai Processing.cs
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
using System.Threading;
using System.Threading.Tasks;
using Comet.Shared;
using Comet.Shared.Comet.Shared;

#endregion

namespace Comet.Game.World.Threading
{
    public sealed class AiProcessor : TimerBase
    {
        public AiProcessor()
            : base(500, "Ai Thread")
        {
        }

        public int ProcessedMonsters { get; private set; }

        protected override async Task<bool> OnElapseAsync()
        {
            var cts = new CancellationTokenSource(2000); // Set a 2-second timeout
            try
            {
                ProcessedMonsters = 0;
                // Wrap your AI processing tasks in a Task.Run to support cancellation
                var aiProcessingTask = Task.Run(async () =>
                {
                    foreach (var map in Kernel.MapManager.GameMaps.Values)
                    {
                        // Check for cancellation before starting each potentially long operation
                        cts.Token.ThrowIfCancellationRequested();
                        ProcessedMonsters += await map.OnTimerAsync();
                    }

                    // Check for cancellation before another potentially long operation
                    cts.Token.ThrowIfCancellationRequested();
                    await Kernel.RoleManager.OnRoleTimerAsync();
                }, cts.Token);

                await aiProcessingTask; // This will throw TaskCanceledException if the task is cancelled
                return true; // Indicates successful completion without timeout
            }
            catch (OperationCanceledException)
            {
                await Log.WriteLogAsync(LogLevel.Warning, "AiProcessing thread maybe got deadlocked");
                return false; // Indicates a timeout occurred
            }
            catch (Exception ex)
            {
                await Log.WriteLogAsync(LogLevel.Error, "AiProcessing::OnElapseAsync encountered an error.");
                await Log.WriteLogAsync(LogLevel.Exception, ex.ToString());
                return false; // Indicates an error occurred
            }
        }
    }
}