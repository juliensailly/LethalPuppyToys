using GameNetcodeStuff;
using UnityEngine;

namespace LethalPuppyToys.Items
{
    /// <summary>
    /// Template for a throwable item like grenades or balls.
    /// Inherits from GrabbableObject and implements throwable mechanics.
    /// Note: You may need to use PhysicsProp or another base class depending on your item type.
    /// </summary>
    public class ExampleThrowableItem : PhysicsProp
    {
        [Header("Throwable Properties")]
        public float explosionRadius = 5f;
        public int damage = 50;
        
        [Header("Effects")]
        public GameObject? explosionPrefab;
        public AudioClip? explosionSound;

        private bool hasExploded = false;
        private PlayerControllerB? playerWhoThrew;

        public override void Start()
        {
            base.Start();
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            
            // Store the player who is using/throwing the item
            if (playerHeldBy != null)
            {
                playerWhoThrew = playerHeldBy;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasExploded)
                return;

            Plugin.Logger.LogInfo($"{itemProperties.itemName} hit {collision.gameObject.name}");
            
            // Add your collision logic here
            // Example: check if hit an enemy, player, or ground
            // You might want to add a delay or velocity check before exploding
        }

        private void Explode()
        {
            if (hasExploded)
                return;

            hasExploded = true;

            // Spawn explosion effect
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // Play explosion sound
            if (explosionSound != null)
            {
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);
            }

            // Deal damage to nearby entities
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var hitCollider in hitColliders)
            {
                // Example: damage enemies
                var enemy = hitCollider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.HitEnemy(damage, playerWhoThrew);
                }

                // Example: damage players
                var player = hitCollider.GetComponent<PlayerControllerB>();
                if (player != null && player != playerWhoThrew)
                {
                    player.DamagePlayer(damage);
                }
            }

            // Destroy the item
            Destroy(gameObject);
        }
    }
}
