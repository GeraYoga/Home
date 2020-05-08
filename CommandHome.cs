using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace GY.Home
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class CommandHome : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer) caller;
            
            if (Home.PlayersController.ContainsKey(player.CSteamID))
            {
                UnturnedChat.Say(player, Home.Instance.Translate("teleportation_in_proc"), Color.red);
                return;
            }
            
            if (!BarricadeManager.tryGetBed(player.CSteamID, out var position, out _))
            {
                UnturnedChat.Say(player, Home.Instance.Translate("bed_not_found"), Color.red);
                return;
            }
            
            var currentSource = new CancellationTokenSource();
            Home.PlayersController.Add(player.CSteamID, currentSource);
            UnturnedChat.Say(player, Home.Instance.Translate("teleport_info", Home.Config.TeleportDelay), Color.green);
            
            Task.Run(async () =>
            {
                await Task.Delay(Home.Config.TeleportDelay * 1000, currentSource.Token);
                player.Player.teleportToLocationUnsafe(position, 0);
                Home.FinishTeleportTask(player.CSteamID);
                UnturnedChat.Say(player, Home.Instance.Translate("teleported_to_bed"), Color.magenta);
                
            }, currentSource.Token);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Home";
        public string Help => "";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>{"gy.home"};
    }
}