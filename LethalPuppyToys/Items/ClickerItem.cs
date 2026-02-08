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
            
            
            noiseRange = 60f;
            minLoudness = 0.6f;
            maxLoudness = 1f;
            minPitch = 0.9f;
            maxPitch = 1f;
            
            noiseAudio.spatialBlend = 1f;
            noiseAudio.rolloffMode = AudioRolloffMode.Linear;
            noiseAudio.maxDistance = 60f;
            
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
                if (distance <= 10f)
                {
                    Plugin.Logger.LogInfo($"Training local player (distance: {distance:F2}m)");
                    
                    Vector3 directionToClicker = clickerPosition - localPlayer.transform.position;
                    
                    if (directionToClicker != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(directionToClicker);
                        localPlayer.transform.rotation = Quaternion.Slerp(
                            localPlayer.transform.rotation, 
                            targetRotation, 
                            0.5f
                        );
                        
                        Plugin.Logger.LogInfo($"Player rotated to face clicker at {clickerPosition}");
                    }
                }
            }
        }
        
        public override void DiscardItem()
        {
            base.DiscardItem();
        }
    }
}
