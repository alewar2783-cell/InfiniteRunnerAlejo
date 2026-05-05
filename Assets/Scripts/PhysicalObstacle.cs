using UnityEngine;

public class PhysicalObstacle : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("How much health to subtract from the player on impact. Set to 1, 2, or 3 based on obstacle size.")]
    public int damageAmount = 1;

    // CRITICAL: Physical collisions can fire multiple times in a single frame as meshes slide against each other.
    // This boolean ensures the obstacle only ever deals damage exactly once per instance.
    private bool hasDealtDamage = false;

    // Use OnCollisionEnter for solid physical impacts instead of OnTriggerEnter.
    // This method only fires if BOTH objects have colliders, and at least one has a Rigidbody,
    // and NEITHER is marked as "Is Trigger".
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Ensure this obstacle hasn't already damaged the player
        if (hasDealtDamage) return;

        // 2. Detect if the object that physically bumped us is the Player
        // Notice we check collision.gameObject, not collision.collider
        if (collision.gameObject.CompareTag("Player"))
        {
            // 3. Lock this obstacle from dealing damage ever again
            hasDealtDamage = true;

            // 4. Safely apply the damage via the GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TakeDamage(damageAmount);
                Debug.Log($"Physical crash! Player took {damageAmount} damage.");
            }
            else
            {
                Debug.LogWarning("PhysicalObstacle could not find a GameManager instance!");
            }

            // Optional: You do NOT want to Destroy(gameObject) here like we did with Triggers, 
            // because destroying the object on frame 1 of a collision kills the satisfying physical "bump".
            // The obstacle will naturally be cleaned up by the ChunkMovement script later when it passes the camera!
        }
    }
}
