using Rocket.API;

namespace GY.Home
{
    public class Config : IRocketPluginConfiguration
    {
        public int TeleportDelay;
        public bool AllowMovement;
        public bool AllowPvP;
        public void LoadDefaults()
        {
            TeleportDelay = 5;
            AllowMovement = true;
            AllowPvP = true;
        }
    }
}