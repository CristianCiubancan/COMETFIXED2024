using Comet.Account.Managers;
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
            var actor = ClientManager.GetClient(AccountIdentity);
            if (actor == null)
            {
                await Log.WriteLogAsync("login", LogLevel.Error, $"[{AccountIdentity}] has not authenticated successfully.");
                return;
            }

            switch (Result)
            {
                case ExchangeResult.AlreadySignedIn:
                case ExchangeResult.ServerFull:
                case ExchangeResult.Success:
                    {
                        // continue login sequence
                        await actor.SendAsync(new MsgConnectEx(actor.Realm.GameIPAddress, actor.Realm.GamePort, Token));
                        await Log.WriteLogAsync("login", LogLevel.Info, $"[{actor.Account.Username}] has authenticated successfully on [{actor.Realm.Name}].");
                        break;
                    }
                case ExchangeResult.KeyError:
                    {
                        await actor.SendAsync(new MsgConnectEx(MsgConnectEx.RejectionCode.ServerBusy));
                        await Log.WriteLogAsync("login", LogLevel.Info, $"[{actor.Account.Username}] failed was not authorized to login on [{actor.Realm.Name}].");
                        break;
                    }
            }            
        }
    }
}