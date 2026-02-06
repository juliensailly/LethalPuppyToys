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
        }

        public override void Start()
        {
            base.Start();
            
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                Plugin.Logger.LogWarning("No AudioSource found on ClickerItem!");
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            if (clickSound == null && audioSource != null && audioSource.clip != null)
            {
                clickSound = audioSource.clip;
            }
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            
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
            }
            else if (clickSound == null)
            {
                Plugin.Logger.LogWarning("Click sound is not assigned!");
            }
        }
        
        public override void DiscardItem()
        {
            base.DiscardItem();
        }
    }
}
