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

            LethalPuppyToysConfig = new Config(Config);
            
            LoadAssetBundle();
            
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
            
             RegisterScrapItems(bundle);
        }

        private void RegisterScrapItems(AssetBundle bundle)
        {
            Item clickerItem = bundle.LoadAsset<Item>("assets/lethalpuppytoys/clickeritem.asset");
            if (clickerItem != null)
            {
                AudioClip clickSound = bundle.LoadAsset<AudioClip>("assets/lethalpuppytoys/clicker-sound.mp3");
                
                Items.ClickerItem clickerComponent = clickerItem.spawnPrefab.AddComponent<Items.ClickerItem>();

                clickerComponent.itemProperties = clickerItem;
                
                if (clickSound != null)
                {
                    var audioSourceOnPrefab = clickerItem.spawnPrefab.GetComponent<AudioSource>();
                    if (audioSourceOnPrefab == null)
                    {
                        audioSourceOnPrefab = clickerItem.spawnPrefab.AddComponent<AudioSource>();
                    }
                    audioSourceOnPrefab.clip = clickSound;
                    audioSourceOnPrefab.playOnAwake = false;
                    audioSourceOnPrefab.spatialBlend = 1f;
                    
                    clickerComponent.Initialize(clickSound, cooldown: 0.2f);
                }
                else
                {
                    Logger.LogWarning("Click sound not found in asset bundle!");
                }
                
                NetworkPrefabs.RegisterNetworkPrefab(clickerItem.spawnPrefab);
                LethalLib.Modules.Items.RegisterScrap(clickerItem, LethalPuppyToysConfig.ClickerItemRarity.Value, Levels.LevelTypes.All);

                clickerComponent.grabbable = true;
            } else
            {
                Logger.LogError("Failed to load ClickerItem from asset bundle!");
            }
        }
    }
}
