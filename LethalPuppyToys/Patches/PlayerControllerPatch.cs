using GameNetcodeStuff;
using HarmonyLib;

namespace LethalPuppyToys.Patches
{
    /// <summary>
    /// Example Harmony patch template.
    /// Patches allow you to modify game behavior without changing the original code.
    /// </summary>
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerPatch
    {
        // Example: Patch the Start method of PlayerControllerB
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPostfix(PlayerControllerB __instance)
        {
            Plugin.Logger.LogInfo($"Player {__instance.playerUsername} started!");
            
            // Add your custom logic here that runs after the player starts
        }

        // Example: Patch when player takes damage
        [HarmonyPatch("DamagePlayer")]
        [HarmonyPrefix]
        private static void DamagePlayerPrefix(PlayerControllerB __instance, int damageNumber)
        {
            if (Plugin.MyConfig.EnableDebugMode.Value)
            {
                Plugin.Logger.LogInfo($"Player {__instance.playerUsername} is taking {damageNumber} damage!");
            }
            
            // Add your custom logic here that runs before the player takes damage
            // Return false to prevent the original method from running
        }
    }
}
