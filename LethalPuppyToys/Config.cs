using BepInEx.Configuration;

namespace LethalPuppyToys
{
    internal class Config
    {
        // General Settings
        public ConfigEntry<bool> EnableDebugMode { get; private set; }
        
        // Example: Item spawn rates
        // Add more config entries as needed for your items
        public ConfigEntry<int> ClickerItemRarity { get; private set; }

        public Config(ConfigFile configFile)
        {
            // General section
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
