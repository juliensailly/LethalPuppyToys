using BepInEx.Configuration;

namespace LethalPuppyToys
{
    internal class Config
    {
        public ConfigEntry<bool> EnableDebugMode { get; private set; }
        
        public ConfigEntry<int> ClickerItemRarity { get; private set; }

        public Config(ConfigFile configFile)
        {
            EnableDebugMode = configFile.Bind(
                "General",
                "EnableDebugMode",
                false,
                "Enable debug logging for troubleshooting."
            );

            ClickerItemRarity = configFile.Bind(
                "Spawn Rates",
                "ClickerItemRarity",
                10,
                "Rarity of the Clicker Item (higher = more common)."
            );
        }
    }
}
