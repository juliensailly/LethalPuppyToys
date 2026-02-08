using System.Collections;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using LethalPuppyToys.Utilities;

namespace LethalPuppyToys.Items
{
    /// <summary>
    /// Clicker item that plays a sound when the left mouse button is clicked.
    /// </summary>
    public class ClickerItem : NoisemakerProp
    {
        private float clickCooldown = 0.2f;
        private float lastClickTime;
        private const float detectionDistance = 30f;

        private AudioClip[]? clickSounds;

        /// <summary>
        /// Initialize the clicker with sound variants and optional cooldown.
        /// </summary>
        public void Initialize(AudioClip[] sounds, float cooldown = 0.2f)
        {
            clickSounds = sounds;
            clickCooldown = cooldown;
        }

        public override void Start()
        {
            base.Start();
            
            if (noiseAudio == null)
            {
                noiseAudio = GetComponent<AudioSource>();
                
                if (noiseAudio == null)
                {
                    Plugin.Logger.LogWarning("No AudioSource found on ClickerItem!");
                    noiseAudio = gameObject.AddComponent<AudioSource>();
                }
            }
            
            if (clickSounds != null && clickSounds.Length > 0)
            {
                noiseSFX = clickSounds;
                noiseSFXFar = clickSounds;
            }
            else if (noiseAudio.clip != null)
            {
                clickSounds = new AudioClip[] { noiseAudio.clip };
                noiseSFX = clickSounds;
                noiseSFXFar = clickSounds;
                Plugin.Logger.LogInfo("Using AudioSource clip as fallback for ClickerItem");
            }
            else
            {
                Plugin.Logger.LogError("No audio clip found for ClickerItem!");
            }
            
            
            
            noiseRange = 100f;
            minLoudness = 1.5f;
            maxLoudness = 2f;
            minPitch = 0.9f;
            maxPitch = 1f;
            
            noiseAudio.spatialBlend = 1f;
            noiseAudio.rolloffMode = AudioRolloffMode.Linear;
            noiseAudio.maxDistance = 100f;
            noiseAudio.volume = 1f;
            
            Plugin.Logger.LogInfo($"ClickerItem initialized - noiseSFX count: {noiseSFX?.Length ?? 0}");
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (buttonDown && Time.time - lastClickTime >= clickCooldown)
            {
                lastClickTime = Time.time;
                PlayClickSoundServerRpc();
                
                if (playerHeldBy != null)
                {
                    RequestPuppyTrainingServerRpc(playerHeldBy.playerClientId, transform.position);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayClickSoundServerRpc()
        {
            PlayClickSoundClientRpc();
        }

        [ClientRpc]
        private void PlayClickSoundClientRpc()
        {
            base.ItemActivate(true, true);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void RequestPuppyTrainingServerRpc(ulong trainerId, Vector3 clickerPosition)
        {
            PuppyTrainingClientRpc(trainerId, clickerPosition);
        }

        [ClientRpc]
        private void PuppyTrainingClientRpc(ulong trainerId, Vector3 clickerPosition)
        {
            PlayerControllerB localPlayer = Helpers.GetLocalPlayer();
            if (localPlayer == null) return;
            
            if (localPlayer.playerClientId != trainerId)
            {
                float distance = Vector3.Distance(localPlayer.transform.position, clickerPosition);
                if (distance <= detectionDistance)
                {
                    Plugin.Logger.LogInfo($"Training local player (distance: {distance:F2}m)");
                    
                    Vector3 directionToClicker = clickerPosition - localPlayer.transform.position;
                    
                    if (directionToClicker != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(directionToClicker);
                        
                        StartCoroutine(SmoothRotatePlayer(localPlayer, targetRotation));
                        
                        Plugin.Logger.LogInfo($"Started rotating player to face clicker at {clickerPosition}");
                    }
                }
            }
        }
        
        /// <summary>
        /// Smoothly rotates a player to face the target rotation over time.
        /// </summary>
        private IEnumerator SmoothRotatePlayer(PlayerControllerB player, Quaternion targetRotation)
        {
            float rotationDuration = 0.3f;
            float elapsedTime = 0f;
            Quaternion startRotation = player.transform.rotation;
            
            while (elapsedTime < rotationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / rotationDuration;
                
                t = t * t * (3f - 2f * t);
                
                player.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                
                yield return null;
            }
            
            player.transform.rotation = targetRotation;
            
            Plugin.Logger.LogInfo("Player rotation animation complete");
        }
        
        public override void DiscardItem()
        {
            base.DiscardItem();
        }
    }
}
