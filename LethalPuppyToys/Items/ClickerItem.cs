using Unity.Netcode;
using UnityEngine;

namespace LethalPuppyToys.Items
{
    /// <summary>
    /// Clicker item that plays a sound when the left mouse button is clicked.
    /// </summary>
    public class ClickerItem : NoisemakerProp
    {
        private float clickCooldown = 0.2f;
        private float lastClickTime;

        private AudioClip? clickSound;

        /// <summary>
        /// Initialize the clicker with a sound and optional cooldown.
        /// </summary>
        public void Initialize(AudioClip sound, float cooldown = 0.2f)
        {
            clickSound = sound;
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
            
            if (clickSound != null)
            {
                noiseSFX = new AudioClip[] { clickSound };
                noiseSFXFar = new AudioClip[] { clickSound };
            }
            else if (noiseAudio.clip != null)
            {
                clickSound = noiseAudio.clip;
                noiseSFX = new AudioClip[] { clickSound };
                noiseSFXFar = new AudioClip[] { clickSound };
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
        
        public override void DiscardItem()
        {
            base.DiscardItem();
        }
    }
}
