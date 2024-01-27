using Comet.Account.Database;
using Comet.Account.Database.Models;
using Comet.Account.States;
using Comet.Network.Packets.Internal;
using System;
using System.Threading.Tasks;

namespace Comet.Account.Packets
{
    public sealed class MsgAccServerGameInformation : MsgAccServerGameInformation<GameServer>
    {
        public override async Task ProcessAsync(GameServer client)
        {
            await BaseRepository.SaveAsync(new DbRealmStatus
            {
                RealmIdentity = client.Realm.RealmID,
                RealmName = client.Realm.Name,
                NewStatus = DbRealm.RealmStatus.Online,
                OldStatus = DbRealm.RealmStatus.Online,
                MaxPlayersOnline = (uint)PlayerCountRecord,
                PlayersOnline = (uint)PlayerCount,
                Time = DateTime.Now
            });
        }
    }
}
