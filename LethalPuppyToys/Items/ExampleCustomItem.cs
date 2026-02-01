using UnityEngine;

namespace LethalPuppyToys.Items
{
    /// <summary>
    /// Template for a basic grabbable item with custom behavior.
    /// Inherit from GrabbableObject to create custom items.
    /// </summary>
    public class ExampleCustomItem : GrabbableObject
    {
        // Audio clips
        [Header("Audio")]
        public AudioClip? grabSound;
        public AudioClip? dropSound;
        public AudioClip? useSound;

        // Custom properties
        [Header("Custom Properties")]
        public float cooldownTime = 2f;
        private float lastUseTime;
        
        private AudioSource? audioSource;

        public override void Start()
        {
            base.Start();
            audioSource = GetComponent<AudioSource>();
            Plugin.Logger.LogInfo($"{itemProperties.itemName} initialized!");
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            
            if (buttonDown && Time.time - lastUseTime >= cooldownTime)
            {
                UseItem();
                lastUseTime = Time.time;
            }
        }

        private void UseItem()
        {
            // Play sound if available
            if (useSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(useSound);
            }

            Plugin.Logger.LogInfo($"{itemProperties.itemName} was used!");
            
            // Add your custom item behavior here
            // Example: spawn effect, trigger animation, etc.
        }

        public override void GrabItem()
        {
            base.GrabItem();
            
            if (grabSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(grabSound);
            }
        }

        public override void DiscardItem()
        {
            base.DiscardItem();
            
            if (dropSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(dropSound);
            }
        }
    }
}

