using UnityEngine;

namespace LethalPuppyToys.Items
{
    /// <summary>
    /// Clicker item that plays a sound when the left mouse button is clicked.
    /// </summary>
    public class ClickerItem : PhysicsProp
    {
        private AudioClip? clickSound;
        private float clickCooldown = 0.2f;
        private float lastClickTime;
        private AudioSource? audioSource;

        /// <summary>
        /// Initialize the clicker with a sound and optional cooldown.
        /// This is called on the prefab, not on instances.
        /// </summary>
        public void Initialize(AudioClip sound, float cooldown = 0.2f)
        {
            clickSound = sound;
            clickCooldown = cooldown;
            Plugin.Logger.LogInfo($"ClickerItem initialized with sound: {sound?.name}");
        }

        public override void Start()
        {
            base.Start();
            
            // Get the AudioSource that should already be on the GameObject
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                Plugin.Logger.LogWarning("No AudioSource found on ClickerItem!");
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // If clickSound wasn't set via Initialize (because this is an instance),
            // get it from the AudioSource.clip that was set on the prefab
            if (clickSound == null && audioSource != null && audioSource.clip != null)
            {
                clickSound = audioSource.clip;
                Plugin.Logger.LogInfo($"ClickerItem loaded sound from AudioSource: {clickSound.name}");
            }
            
            Plugin.Logger.LogInfo($"ClickerItem '{itemProperties.itemName}' started!");
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            
            // buttonDown is true when the button is pressed down (not held)
            if (buttonDown && Time.time - lastClickTime >= clickCooldown)
            {
                PlayClickSound();
                lastClickTime = Time.time;
            }
        }

        private void PlayClickSound()
        {
            if (clickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(clickSound);
                Plugin.Logger.LogDebug($"Clicker sound played!");
            }
            else if (clickSound == null)
            {
                Plugin.Logger.LogWarning("Click sound is not assigned!");
            }
        }
        
        public override void DiscardItem()
        {
            base.DiscardItem();
            Plugin.Logger.LogDebug("Clicker item discarded.");
        }
    }
}
