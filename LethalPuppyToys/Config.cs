using BepInEx.Configuration;

namespace LethalPuppyToys
{
    internal class Config
    {
        // General Settings
        public ConfigEntry<bool> EnableDebugMode { get; private set; }
        
        // Example: Item spawn rates
        // Add more config entries as needed for your items
        // public ConfigEntry<int> ExampleItemRarity { get; private set; }

        public Config(ConfigFile configFile)
        {
            // General section
            EnableDebugMode = configFile.Bind(
                "General",
                "EnableDebugMode",
                false,
                "Enable debug logging for troubleshooting."
            );

            // Example: Item spawn rates section
            /*
            ExampleItemRarity = configFile.Bind(
                "Spawn Rates",
                "ExampleItemRarity",
                10,
                "Rarity of the Example Item (higher = more common)."
            );
            */
        }
    }
}
