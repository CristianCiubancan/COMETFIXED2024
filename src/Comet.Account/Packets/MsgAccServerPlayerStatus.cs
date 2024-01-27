using Comet.Account.Database.Models;
using Comet.Account.Database.Repositories;
using Comet.Account.States;
using Comet.Network.Packets.Internal;
using Comet.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace Comet.Account.Packets
{
    public sealed class MsgAccServerPlayerStatus : MsgAccServerPlayerStatus<GameServer>
    {
        public override async Task ProcessAsync(GameServer client)
        {
            DbRealm realm = Kernel.Realms.Values.FirstOrDefault(x => x.Name.Equals(ServerName));
            if (realm == null)
            {
                await Log.WriteLogAsync(LogLevel.Warning, $"Invalid server name [{ServerName}] tried to update data from [{client.IPAddress}].");
                return;
            }

            if (realm.Server == null)
            {
                await Log.WriteLogAsync(LogLevel.Warning, $"{ServerName} is not connected and tried to update player status from [{client.IPAddress}].");
                return;
            }

            if (Count == 0)
                return;

            foreach (var info in Status)
            {
                if (info.Online)
                {
                    DbAccount account = await AccountsRepository.FindAsync(info.Identity);
                    if (account == null)
                    {
                        await client.SendAsync(new MsgAccServerCmd
                        {
                            Action = MsgAccServerCmd<GameServer>.ServerAction.Disconnect,
                            AccountIdentity = info.Identity
                        });
                        return;
                    }

                    if (account.StatusID == 5 || account.StatusID == 4)
                    {
                        await client.SendAsync(new MsgAccServerCmd
                        {
                            Action = MsgAccServerCmd<GameServer>.ServerAction.Disconnect,
                            AccountIdentity = info.Identity
                        });
                        return;
                    }

                    Kernel.Players.TryAdd(info.Identity, new Player
                    {
                        Account = account,
                        AccountIdentity = account.AccountID,
                        Realm = realm
                    });
                }
                else
                {
                    Kernel.Players.TryRemove(info.Identity, out _);
                }
            }
        }
    }
}