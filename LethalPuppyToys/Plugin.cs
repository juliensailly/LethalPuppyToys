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
        private const string GUID = "juu.lethalpuppytoys";
        private const string NAME = "LethalPuppyToys";
        private const string VERSION = "1.0.0";

        public static Plugin Instance { get; private set; } = null!;
        internal static new ManualLogSource Logger { get; private set; } = null!;
        internal static Config LethalPuppyToysConfig { get; private set; } = null!;
        
        private readonly Harmony harmony = new Harmony(GUID);

        private void Awake()
        {
            Instance = this;
            Logger = base.Logger;

            // Initialize config
            LethalPuppyToysConfig = new Config(Config);
            
            // Load asset bundle (uncomment when you have one)
            LoadAssetBundle();
            
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
            
             RegisterScrapItems(bundle);
        }

        private void RegisterScrapItems(AssetBundle bundle)
        {
            // Load the clicker item
            Item clickerItem = bundle.LoadAsset<Item>("ClickerItem");
            if (clickerItem != null)
            {
                // Load the click sound from the asset bundle
                AudioClip clickSound = bundle.LoadAsset<AudioClip>("clicker-sound.mp3");
                
                // Check if there's already a PhysicsProp or GrabbableObject component
                var existingPhysicsProp = clickerItem.spawnPrefab.GetComponent<PhysicsProp>();
                var existingGrabbable = clickerItem.spawnPrefab.GetComponent<GrabbableObject>();
                Items.ClickerItem clickerComponent;
                
                if (existingPhysicsProp != null && !(existingPhysicsProp is Items.ClickerItem))
                {
                    // Replace the existing PhysicsProp with ClickerItem
                    Logger.LogInfo("Replacing existing PhysicsProp with ClickerItem");
                    Object.DestroyImmediate(existingPhysicsProp);
                    clickerComponent = clickerItem.spawnPrefab.AddComponent<Items.ClickerItem>();
                }
                else if (existingGrabbable != null && !(existingGrabbable is Items.ClickerItem))
                {
                    // Replace the existing GrabbableObject with ClickerItem
                    Logger.LogInfo("Replacing existing GrabbableObject with ClickerItem");
                    Object.DestroyImmediate(existingGrabbable);
                    clickerComponent = clickerItem.spawnPrefab.AddComponent<Items.ClickerItem>();
                }
                else if (existingPhysicsProp == null && existingGrabbable == null)
                {
                    // Just add the ClickerItem component
                    clickerComponent = clickerItem.spawnPrefab.AddComponent<Items.ClickerItem>();
                }
                else
                {
                    // Already has ClickerItem
                    clickerComponent = (Items.ClickerItem)existingPhysicsProp ?? (Items.ClickerItem)existingGrabbable;
                }
                
                // Copy the itemProperties reference (critical!)
                clickerComponent.itemProperties = clickerItem;
                
                // Initialize the clicker with the sound (configure in code)
                if (clickSound != null)
                {
                    clickerComponent.Initialize(clickSound, cooldown: 0.2f);
                    Logger.LogInfo($"Click sound loaded: {clickSound.name}");
                }
                else
                {
                    Logger.LogWarning("Click sound not found in asset bundle!");
                }
                
                NetworkPrefabs.RegisterNetworkPrefab(clickerItem.spawnPrefab);
                LethalLib.Modules.Items.RegisterScrap(clickerItem, LethalPuppyToysConfig.ClickerItemRarity.Value, Levels.LevelTypes.All);
                
                Logger.LogInfo($"Registered item: {clickerItem.itemName}");
            } else
            {
                Logger.LogError("Failed to load ClickerItem from asset bundle!");
            }
        }
    }
}
