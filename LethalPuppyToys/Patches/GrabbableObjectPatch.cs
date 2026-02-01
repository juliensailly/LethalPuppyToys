using HarmonyLib;

namespace LethalPuppyToys.Patches
{
    /// <summary>
    /// Example patch for modifying item behavior in the game.
    /// </summary>
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatch
    {
        // Example: Modify what happens when any item is grabbed
        [HarmonyPatch("GrabItem")]
        [HarmonyPostfix]
        private static void GrabItemPostfix(GrabbableObject __instance)
        {
            if (Plugin.MyConfig.EnableDebugMode.Value)
            {
                Plugin.Logger.LogInfo($"Item grabbed: {__instance.itemProperties.itemName}");
            }
            
            // Add custom logic for when items are grabbed
        }

        // Example: Modify item drop behavior
        [HarmonyPatch("DiscardItem")]
        [HarmonyPostfix]
        private static void DiscardItemPostfix(GrabbableObject __instance)
        {
            if (Plugin.MyConfig.EnableDebugMode.Value)
            {
                Plugin.Logger.LogInfo($"Item discarded: {__instance.itemProperties.itemName}");
            }
            
            // Add custom logic for when items are dropped
        }
    }
}
