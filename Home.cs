using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace GY.Home
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class Home : RocketPlugin<Config>
    {
        public static Home Instance;
        public static Config Config;
        public static readonly List<CSteamID> PlayersController = new List<CSteamID>();

        protected override void Load()
        {
            DamageTool.damagePlayerRequested += DamageToolOnDamagePlayerRequested;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += UnturnedPlayerEventsOnOnPlayerUpdatePosition;
            U.Events.OnPlayerDisconnected += EventsOnOnPlayerDisconnected;
            Instance = this;
            Config = Instance.Configuration.Instance;
        }

        private static void EventsOnOnPlayerDisconnected(UnturnedPlayer player)
        {
            if (PlayersController.Contains(player.CSteamID))
            {
                FinishTeleportTask(player.CSteamID);
            }
        }


        public override TranslationList DefaultTranslations => new TranslationList
        {
            {"bed_not_found", "Ваша кровать не найдена!"},
            {"teleported_to_bed", "Вы телепортировались к своей кровати!"},
            {"pvp_mode", "Вы получили урон, телепортация отменена!"},
            {"movement_detected", "Телепортация отменена, вы двигались!"},
            {"teleportation_in_proc", "Процесс телепортации уже активен..."},
            {"teleport_info", "Запрос на телепортацию к кровате получен, ждте {0} секунд!"}
        };

        private void UnturnedPlayerEventsOnOnPlayerUpdatePosition(UnturnedPlayer player, Vector3 position)
        {
            if (Config.AllowMovement) return;
            if (!PlayersController.Contains(player.CSteamID)) return;
            FinishTeleportTask(player.CSteamID);
            UnturnedChat.Say(player, Translate("movement_detected"), Color.yellow);
        }
        

        private void DamageToolOnDamagePlayerRequested(ref DamagePlayerParameters parameters, ref bool allow)
        {
            var target = parameters.player.channel.owner.playerID.steamID;
            if (Config.AllowPvP) return;
            if (!PlayersController.Contains(target)) return;
            FinishTeleportTask(target);
            UnturnedChat.Say(target, Translate("pvp_mode"), Color.yellow);
        }

        public static void FinishTeleportTask(CSteamID target)
        {
            PlayersController.Remove(target);
        }

        protected override void Unload()
        {
            DamageTool.damagePlayerRequested -= DamageToolOnDamagePlayerRequested;
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= UnturnedPlayerEventsOnOnPlayerUpdatePosition;
            U.Events.OnPlayerDisconnected -= EventsOnOnPlayerDisconnected;
            Instance = null;
            PlayersController.AsParallel().ForAll(FinishTeleportTask);
            PlayersController.Clear();
        }
    }
}