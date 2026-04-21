using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Tooltip("How much health to subtract from the player on impact.")]
    public int damageAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Detect if the object that entered the trigger is the Player
        if (other.CompareTag("Player"))
        {
            // Apply damage via GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TakeDamage(damageAmount);
            }
            else
            {
                Debug.LogWarning("No GameManager instance found!");
            }
            
            // Disable the collider immediately so it doesn't trigger multiple times in one frame/crash
            GetComponent<Collider>().enabled = false;

            // Optional: Play a crash particle effect or sound effect here
            
            // Destroy the obstacle to clean it up visually
            Destroy(gameObject);
        }
    }
}
