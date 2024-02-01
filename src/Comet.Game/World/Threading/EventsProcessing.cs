﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Events Processing.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Comet.Core;
using Comet.Game.States;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Events;
using Comet.Game.States.Items;
using Comet.Game.States.NPCs;
using Comet.Game.World.Managers;
using Comet.Shared;
using Comet.Shared.Comet.Shared;

#endregion

namespace Comet.Game.World.Threading
{
    public sealed class EventsProcessing : TimerBase
    {
        private TimeOut m_rankingBroadcast = new TimeOut(10);
        private ConcurrentDictionary<GameEvent.EventType, GameEvent> m_events = new ConcurrentDictionary<GameEvent.EventType, GameEvent>();
        //private ConcurrentDictionary<uint, QueuedAction> m_queuedActions = new ConcurrentDictionary<uint, QueuedAction>();
        private List<QueuedAction> m_queuedActions = new List<QueuedAction>();

        public EventsProcessing()
            : base(500, "EventsProcessing")
        {
            m_rankingBroadcast.Update();
        }

        protected override async Task<bool> OnElapseAsync()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(2000); // Set timeout for 2 seconds.

            try
            {
                // Wrap potentially long-running operations in a task to allow for cancellation.
                var task = Task.Run(async () =>
                {
                    await Kernel.PigeonManager.OnTimerAsync();

                    bool ranking = m_rankingBroadcast.ToNextTime();
                    foreach (var dynaNpc in Kernel.RoleManager.QueryRoleByType<DynamicNpc>())
                    {
                        if (dynaNpc.IsGoal() || cts.IsCancellationRequested)
                            continue;

                        await dynaNpc.CheckFightTimeAsync();
                        //TODO: implement cancelation like: await dynaNpc.CheckFightTimeAsync(cts.Token);
                        if (ranking && m_events.Values.All(x => x.Map?.Identity != dynaNpc.MapIdentity))
                            await dynaNpc.BroadcastRankingAsync();
                            // TODO: implement cancelation like: await dynaNpc.BroadcastRankingAsync(cts.Token);
                    }

                    foreach (var @event in m_events.Values)
                    {
                        if (@event.ToNextTime() && !cts.IsCancellationRequested)
                            await @event.OnTimerAsync();
                            // TODO: implement cancelation like: await @event.OnTimerAsync(cts.Token);
                    }

                    // Process queued actions, considering cancellation token as well.
                    for (int i = m_queuedActions.Count - 1; i >= 0; i--)
                    {
                        if (cts.IsCancellationRequested)
                            break;

                        var action = m_queuedActions[i];
                        Character user = Kernel.RoleManager.GetUser(action.UserIdentity);
                        if (action.CanBeExecuted && user != null)
                        {
                            Item item = null;
                            if (user.InteractingItem != 0)
                            {
                                item = user.UserPackage.FindByIdentity(user.InteractingItem);
                            }
                            Role role = null;
                            if (user.InteractingNpc != 0)
                            {
                                role = Kernel.RoleManager.GetRole(user.InteractingNpc);
                            }

                            await GameAction.ExecuteActionAsync(action.Action, user, role, item, "");
                            m_queuedActions.RemoveAt(i); m_queuedActions.RemoveAt(i);
                        }
                    }
                }, cts.Token);

                await task;
                return !cts.IsCancellationRequested;
            }
            catch (TaskCanceledException)
            {
                // Handle the timeout scenario.
                await Log.WriteLogAsync(LogLevel.Info, "EventsProcessing thread maybe got deadlocked");
                return false;
            }
            finally
            {
                cts.Dispose();
            }
        }
        protected override async Task OnStartAsync()
        {
            await RegisterEventAsync(new TimedGuildWar());
            await RegisterEventAsync(new LineSkillPk());
            await RegisterEventAsync(new ArenaQualifier());
            await RegisterEventAsync(new QuizShow());
            await RegisterEventAsync(new FamilyWar());
            await RegisterEventAsync(new GuildContest());

            await base.OnStartAsync();
        }

        public async Task<bool> RegisterEventAsync(GameEvent @event)
        {
            if (m_events.ContainsKey(@event.Identity))
                return false;

            if (await @event.CreateAsync())
            {
                m_events.TryAdd(@event.Identity, @event);
                return true;
            }
            return false;
        }

        public void RemoveEvent(GameEvent.EventType type)
        {
            m_events.TryRemove(type, out _);
        }

        public T GetEvent<T>() where T : GameEvent
        {
            return m_events.Values.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
        }

        public GameEvent GetEvent(GameEvent.EventType type) => m_events.TryGetValue(type, out var ev) ? ev : null;

        public GameEvent GetEvent(uint idMap)
        {
            return m_events.Values.FirstOrDefault(x => x.Map?.Identity == idMap);
        }

        public bool QueueAction(QueuedAction action)
        {
            //return m_queuedActions.TryAdd(action.UserIdentity, action);
            m_queuedActions.Add(action);
            return true;
        }
    }
}