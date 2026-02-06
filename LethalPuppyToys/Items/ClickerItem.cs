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
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
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
