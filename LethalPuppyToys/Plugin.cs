using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Modules;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LethalPuppyToys
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(LethalLib.Plugin.ModGUID, BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = "juliensailly.lethalpuppytoys";
        private const string NAME = "LethalPuppyToys";
        private const string VERSION = "1.0.0";

        public static Plugin Instance { get; private set; } = null!;
        internal static new ManualLogSource Logger { get; private set; } = null!;
        internal static Config MyConfig { get; private set; } = null!;
        
        private readonly Harmony harmony = new Harmony(GUID);

        private void Awake()
        {
            Instance = this;
            Logger = base.Logger;
            
            // Initialize config
            MyConfig = new Config(Config);
            
            // Load asset bundle (uncomment when you have one)
            // LoadAssetBundle();
            
            // Apply Harmony patches
            harmony.PatchAll();
            
            Logger.LogInfo($"{NAME} v{VERSION} is loaded!");
        }

        private void LoadAssetBundle()
        {
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assetBundlePath = Path.Combine(assemblyLocation!, "lethalpuppytoys");
            
            AssetBundle bundle = AssetBundle.LoadFromFile(assetBundlePath);
            if (bundle == null)
            {
                Logger.LogError("Failed to load asset bundle!");
                return;
            }

            Logger.LogInfo("Asset bundle loaded successfully!");
            
            // Example: Register scrap items
            // RegisterScrapItems(bundle);
        }

        private void RegisterScrapItems(AssetBundle bundle)
        {
            // Example template for registering scrap items
            // Uncomment and modify when you have items to register
            
            /*
            Item exampleItem = bundle.LoadAsset<Item>("Assets/Items/ExampleItem.asset");
            if (exampleItem != null)
            {
                // Add custom behavior if needed
                // exampleItem.spawnPrefab.AddComponent<CustomItemBehavior>();
                
                NetworkPrefabs.RegisterNetworkPrefab(exampleItem.spawnPrefab);
                Items.RegisterScrap(exampleItem, MyConfig.ExampleItemRarity.Value, Levels.LevelTypes.All);
                
                Logger.LogInfo($"Registered item: {exampleItem.itemName}");
            }
            */
        }
    }
}
