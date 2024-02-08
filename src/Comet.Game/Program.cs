// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Program.cs
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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Game.Internal;
using Comet.Game.Packets;
using Comet.Game.States.Items;
using Comet.Game.World;
using Comet.Network.RPC;
using Comet.Network.Security;
using Comet.Shared;
using Comet.Shared.Models;

#endregion

namespace Comet.Game
{
    /// <summary>
    ///     The game server listens for authentication players with a valid access token from
    ///     the account server, and hosts the game world. The game world in this project has
    ///     been simplified into a single server executable. For an n-server distributed
    ///     systems implementation of a Conquer Online server, see Chimera.
    /// </summary>
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Log.DefaultFileName = "GameServer";

            // Copyright notice may not be commented out. If adding your own copyright or
            // claim of ownership, you may include a second copyright above the existing
            // copyright. Do not remove completely to comply with software license. The
            // project name and version may be removed or changed.
            Console.Title = @"Comet, Game Server";
            Console.WriteLine();
            await Log.WriteLogAsync(LogLevel.Info, "  Comet: Game Server");
            await Log.WriteLogAsync(LogLevel.Info, $"  Copyright 2018-{DateTime.Now:yyyy} Gareth Jensen \"Spirited\"");
            await Log.WriteLogAsync(LogLevel.Info, "  All Rights Reserved");
            Console.WriteLine();

            // Read configuration file and command-line arguments
            var config = new ServerConfiguration(args);
            if (!config.Valid)
            {
                await Log.WriteLogAsync(LogLevel.Error, "Invalid server configuration file");
                return;
            }

            Kernel.Configuration = config.GameNetwork;
            AccountClient.Configuration = config.RpcNetwork;

            // Initialize the database
            await Log.WriteLogAsync(LogLevel.Info, "Initializing server...");
            MsgConnect.StrictAuthentication = config.Authentication.StrictAuthPass;
            ServerDbContext.Configuration = config.Database;
            ServerDbContext.Initialize();
            if (!await ServerDbContext.PingAsync())
            {
                await Log.WriteLogAsync(LogLevel.Error, "Invalid database configuration");
                return;
            }

            // Start background services (needed before init)
            var tasks = new List<Task>
            {
                Kernel.Services.Randomness.StartAsync(CancellationToken.None),
                DiffieHellman.ProbablePrimes.StartAsync(CancellationToken.None),
                Kernel.Services.Processor.StartAsync(CancellationToken.None)
            };
            Task.WaitAll(tasks.ToArray());

            if (!await Kernel.StartupAsync().ConfigureAwait(true))
            {
                await Log.WriteLogAsync(LogLevel.Error, "Could not load database related stuff");
                return;
            }

            // start remaining background services
            tasks = new List<Task>
            {
            };
            Task.WaitAll(tasks.ToArray());

            // Start the game server listener
            var server = new Server(config);
            _ = server.StartAsync(config.GameNetwork.Port, config.GameNetwork.IPAddress)
                .ConfigureAwait(false);

            // Output all clear and wait for user input
            await Log.WriteLogAsync(LogLevel.Info, "Listening for new connections");
            Console.WriteLine();

            bool result = await CommandCenterAsync();
            if (!result)
                await Log.WriteLogAsync(LogLevel.Error, "Game server has exited without success.");

            await Kernel.CloseAsync();
        }

        private static async Task<bool> CommandCenterAsync()
        {
            while (true)
            {
                string text = Console.ReadLine();

                if (string.IsNullOrEmpty(text))
                    continue;

                if (text == "exit")
                {
                    await Log.WriteLogAsync(LogLevel.Warning, "Server will shutdown...");
                    return true;
                }

                string[] full = text.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                if (full.Length <= 0)
                    continue;

                switch (full[0].ToLower())
                {
                    case "/players":

                        break;

                    case "/analytics":
                    {
                        await Kernel.SystemThread.DoAnalyticsAsync();
                        break;
                    }

                    case "/broadcast":
                    {
                        string[] bc = text.Split(new[] {" "}, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (bc.Length < 2)
                            break;
                        await Kernel.RoleManager.BroadcastMsgAsync(bc[1], MsgTalk.TalkChannel.Center);
                        break;
                    }

                    case "/createbots":
                    {
                        if (!int.TryParse(full[1], out int amount))
                            break;

                        if (!int.TryParse(full[2], out int initialId))
                            break;

                        for (int i = 0; i < amount; i++, initialId++)
                        {
                            int prof = 40;
                            uint mesh = 11003;

                            DbPointAllot allot = Kernel.RoleManager.GetPointAllot((ushort) (prof / 10), 1) ??
                                                 new DbPointAllot
                                                 {
                                                     Strength = 4,
                                                     Agility = 6,
                                                     Vitality = 12,
                                                     Spirit = 0
                                                 };

                            DbCharacter user = new DbCharacter
                            {
                                Name = $"ImmaBot{i:0000}",
                                MapID = 1002,
                                Mate = 0,
                                Mesh = mesh,
                                X = 430,
                                Y = 478,
                                Strength = allot.Strength,
                                Agility = allot.Agility,
                                Vitality = allot.Vitality,
                                Spirit = allot.Spirit,
                                HealthPoints =
                                    (ushort)(allot.Strength * 3
                                             + allot.Agility * 3
                                             + allot.Spirit * 3
                                             + allot.Vitality * 24),
                                ManaPoints = (ushort)(allot.Spirit * 5),
                                Registered = DateTime.Now,
                                ExperienceMultiplier = 5,
                                ExperienceExpires = DateTime.Now.AddHours(1),
                                HeavenBlessing = DateTime.Now.AddDays(30),
                                AutoAllot = 1,
                                Silver = 1000,
                                Level = 1,
                                Profession = (byte) prof,
                                AccountIdentity = (uint) initialId
                            };

                            if (!await BaseRepository.SaveAsync(user))
                            {
                                await Log.WriteLogAsync(LogLevel.Error, $"Error saving bot {i}");
                                break;
                            }

                            await BaseRepository.SaveAsync(new DbMagic
                            {
                                OwnerId = user.Identity,
                                Type = 8001,
                                Level = 3
                            });

                            var items = new List<DbItem>
                            {
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 500329, Position = 4, Magic3 = 6, Gem1 = 13,
                                    Gem2 = 13, Amount = 7099, AmountLimit = 7099
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 1050002, Position = 5, Amount = 50000,
                                    AmountLimit = 50000
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 1050002, Position = 0, Amount = 50000,
                                    AmountLimit = 50000
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 1050002, Position = 0, Amount = 50000,
                                    AmountLimit = 50000
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 1050002, Position = 0, Amount = 50000,
                                    AmountLimit = 50000
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 1050002, Position = 0, Amount = 50000,
                                    AmountLimit = 50000
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 113109, Position = 1, Magic3 = 6, Gem1 = 13,
                                    Gem2 = 13, Amount = 7099, AmountLimit = 7099
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 133109, Position = 3, Magic3 = 6, Gem1 = 13,
                                    Gem2 = 13, Amount = 7099, AmountLimit = 7099
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 120249, Position = 2, Magic3 = 6, Gem1 = 13,
                                    Gem2 = 13, Amount = 7099, AmountLimit = 7099
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 150249, Position = 6, Magic3 = 6, Gem1 = 13,
                                    Gem2 = 13, Amount = 7099, AmountLimit = 7099
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 160249, Position = 8, Magic3 = 6, Gem1 = 13,
                                    Gem2 = 13, Amount = 7099, AmountLimit = 7099
                                },
                                new DbItem
                                {
                                    PlayerId = user.Identity, Type = 2100075, Position = 7, Amount = 65535,
                                    AmountLimit = 65535
                                },
                            };

                            await BaseRepository.SaveAsync(items);

                            await Log.WriteLogAsync(LogLevel.Debug,
                                $"Bot[{user.Name}:{user.Identity}] has been created");
                        }

                        break;
                    }
                }
            }
        }

        private static async Task ConvertItemsAsync()
        {
            await using ServerDbContext ctx = new ServerDbContext();
            await using StreamWriter writer = new StreamWriter("cq_item_convert.sql", false, Encoding.ASCII);

            await writer.WriteLineAsync(
                $"##############################################################################");
            await writer.WriteLineAsync($"# ");
            await writer.WriteLineAsync($"# Players items exportation and converting tool");
            await writer.WriteLineAsync($"# {Environment.CurrentDirectory}");
            await writer.WriteLineAsync($"# {Environment.UserName} - {DateTime.Now:U}");
            await writer.WriteLineAsync($"# ");
            await writer.WriteLineAsync(
                $"##############################################################################");

            int count = 0;
            DataTable oldItems = await ctx.SelectAsync("SELECT * FROM cq_item_old");
            foreach (DataRow row in oldItems.Rows)
            {
                uint type = uint.Parse(row["type"].ToString());
                byte newColor = byte.Parse(row["color"].ToString());
                if (Item.IsShield(type) || Item.IsArmor(type) || Item.IsHelmet(type))
                {
                    uint oldType = type;
                    int color = (int) (type % 1000 / 100);
                    if (color > 1)
                        newColor = (byte) Math.Max(3, color);
                    type = (uint) (type - color * 100);
                    _ = Log.GmLogAsync($"ItemColorChangeType",
                        $"PlayerId: {row["player_id"]}, OwnerId: {row["owner_id"]}, OldType: {oldType}, NewType: {type}");
                }

                Dictionary<string, string> kvp = new Dictionary<string, string>();
                kvp.Add("`id`", row["id"].ToString());
                kvp.Add("`type`", type.ToString());
                kvp.Add("`owner_id`", row["owner_id"].ToString());
                kvp.Add("`player_id`", row["player_id"].ToString());
                kvp.Add("`amount`", row["amount"].ToString());
                kvp.Add("`amount_limit`", row["amount_limit"].ToString());
                kvp.Add("`ident`", row["ident"].ToString());
                kvp.Add("`position`", row["position"].ToString());
                kvp.Add("`gem1`", row["gem1"].ToString());
                kvp.Add("`gem2`", row["gem2"].ToString());
                kvp.Add("`magic1`", row["magic1"].ToString());
                kvp.Add("`magic2`", row["magic2"].ToString());
                kvp.Add("`magic3`", row["magic3"].ToString());
                kvp.Add("`data`", row["data"].ToString());
                kvp.Add("`reduce_dmg`", row["reduce_dmg"].ToString());
                kvp.Add("`add_life`", row["add_life"].ToString());
                kvp.Add("`anti_monster`", row["anti_monster"].ToString());
                kvp.Add("`chk_sum`", row["chk_sum"].ToString());
                kvp.Add("`SpecialFlag`", row["SpecialFlag"].ToString());
                kvp.Add("`color`", newColor.ToString());
                kvp.Add("`Addlevel_exp`", row["Addlevel_exp"].ToString());
                kvp.Add("`monopoly`", row["monopoly"].ToString());

                string query =
                    $"INSERT INTO `cq_item` ({string.Join(",", kvp.Keys)}) VALUES ({string.Join(",", kvp.Values)});";

                await writer.WriteLineAsync(query);
                count++;
            }

            writer.Close();
            await Log.WriteLogAsync(LogLevel.Debug, $"Converted items: {count}");
        }
    }
}