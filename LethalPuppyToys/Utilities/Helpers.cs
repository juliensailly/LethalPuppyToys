using System.Collections.Generic;
using GameNetcodeStuff;
using LethalLib.Modules;
using Unity.Netcode;
using UnityEngine;

namespace LethalPuppyToys.Utilities
{
    /// <summary>
    /// Utility class for helper functions and extensions.
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        /// Gets the local player instance.
        /// </summary>
        public static PlayerControllerB? GetLocalPlayer()
        {
            return GameNetworkManager.Instance?.localPlayerController;
        }

        /// <summary>
        /// Checks if the local player is the host.
        /// </summary>
        public static bool IsHost()
        {
            return NetworkManager.Singleton?.IsHost ?? false;
        }

        /// <summary>
        /// Spawns an object with networking support.
        /// </summary>
        public static GameObject SpawnNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject spawnedObject = Object.Instantiate(prefab, position, rotation);
            spawnedObject.GetComponent<NetworkObject>()?.Spawn();
            return spawnedObject;
        }

        /// <summary>
        /// Gets all enemies in the current level.
        /// </summary>
        public static EnemyAI[] GetAllEnemies()
        {
            return Object.FindObjectsOfType<EnemyAI>();
        }

        /// <summary>
        /// Gets all items in the current level.
        /// </summary>
        public static GrabbableObject[] GetAllItems()
        {
            return Object.FindObjectsOfType<GrabbableObject>();
        }

        /// <summary>
        /// Logs a message if debug mode is enabled.
        /// </summary>
        public static void DebugLog(string message)
        {
            if (Plugin.LethalPuppyToysConfig.EnableDebugMode.Value)
            {
                Plugin.Logger.LogInfo($"[DEBUG] {message}");
            }
        }

        public static List<PlayerControllerB> GetAllPlayers()
        {
            return new List<PlayerControllerB>(StartOfRound.Instance.allPlayerScripts);
        }

        public static List<PlayerControllerB> GetAllPlayerInRangeOfPlayer(PlayerControllerB trainer, float range)
        {
            List<PlayerControllerB> puppies = new List<PlayerControllerB>();
            foreach (PlayerControllerB player in GetAllPlayers())
            {
                if (player != trainer && Vector3.Distance(trainer.transform.position, player.transform.position) <= range)
                {
                    puppies.Add(player);
                }
            }

            return puppies;
        }
    }
}
