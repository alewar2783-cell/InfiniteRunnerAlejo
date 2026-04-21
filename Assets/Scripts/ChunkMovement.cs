using UnityEngine;

public class ChunkMovement : MonoBehaviour
{
    [Header("Cleanup Settings")]
    [Tooltip("The Z coordinate (behind the player) where this chunk will be destroyed to free memory.")]
    public float destroyDistance = -50f;

    private void Update()
    {
        // 1. Validate that GameManager exists to prevent errors
        if (GameManager.Instance != null && GameManager.Instance.globalWorldSpeed > 0f)
        {
            // 2. Move the chunk towards the player (negative Z axis)
            // Multiplying by Time.deltaTime ensures the movement is smooth and frame-rate independent
            float movementSpeed = GameManager.Instance.globalWorldSpeed;
            transform.Translate(Vector3.back * movementSpeed * Time.deltaTime, Space.World);
        }

        // 3. Self-destruction check
        // Once the chunk is completely out of the camera's view behind the player, destroy it.
        // E.g., if player is at Z = 0, catching chunks at Z < -50 ensures they are safely invisible.
        if (transform.position.z < destroyDistance)
        {
            // To ensure the spawner's list can gracefully handle nulls, ChunkSpawner will automatically
            // clean up missing items from its Tracking list, keeping separation of concerns clean.
            Destroy(gameObject);
        }
    }
}
