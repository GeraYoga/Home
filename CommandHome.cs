using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            
            if (Home.PlayersController.Contains(player.CSteamID))
            {
                UnturnedChat.Say(player, Home.Instance.Translate("teleportation_in_proc"), Color.red);
                return;
            }

            if (!BarricadeManager.tryGetBed(player.CSteamID, out _, out _))
            {
                UnturnedChat.Say(player, Home.Instance.Translate("bed_not_found"), Color.red);
                return;
            }
            
            Home.PlayersController.Add(player.CSteamID);

            Task.Run(async () =>
            {
                UnturnedChat.Say(player, Home.Instance.Translate("teleport_info", Home.Config.TeleportDelay), Color.green);
                await Task.Delay(Home.Config.TeleportDelay * 1000);
                if(!Home.PlayersController.Contains(player.CSteamID)) return;
                Home.FinishTeleportTask(player.CSteamID);
                
                if (player.Player.teleportToBed())
                {
                    UnturnedChat.Say(player, Home.Instance.Translate("teleported_to_bed"), Color.cyan);
                    return;
                }
                
                UnturnedChat.Say(player, Home.Instance.Translate("bed_not_found"), Color.red);
            });

        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Home";
        public string Help => "";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>{"gy.home"};
    }
}