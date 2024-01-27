using Comet.Account.States;
using Comet.Network.Packets.Internal;
using Comet.Shared;
using System.Threading.Tasks;

namespace Comet.Account.Packets
{
    public sealed class MsgAccServerLoginExchangeEx : MsgAccServerLoginExchangeEx<GameServer>
    {
        public override async Task ProcessAsync(GameServer client)
        {
            if (!Kernel.Clients.TryGetValue(AccountIdentity, out var player))
                return;

            switch (Result)
            {
                case ExchangeResult.AlreadySignedIn:
                case ExchangeResult.ServerFull:
                case ExchangeResult.Success:
                    {
                        // continue login sequence
                        await player.SendAsync(new MsgConnectEx(player.Realm.GameIPAddress, player.Realm.GamePort, Token));
                        await Log.WriteLogAsync("login", LogLevel.Info, $"[{player.Account.Username}] has authenticated successfully on [{player.Realm.Name}].");
                        break;
                    }
                case ExchangeResult.KeyError:
                    {
                        await player.SendAsync(new MsgConnectEx(MsgConnectEx.RejectionCode.ServerBusy));
                        await Log.WriteLogAsync("login", LogLevel.Info, $"[{player.Account.Username}] failed was not authorized to login on [{player.Realm.Name}].");
                        break;
                    }
            }            
        }
    }
}