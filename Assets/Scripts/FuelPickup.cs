using UnityEngine;

public class FuelPickup : MonoBehaviour
{
    [Tooltip("How many sections/points of fuel to restore when collected.")]
    public int fuelAmountToRestore = 2;

    private void OnTriggerEnter(Collider other)
    {
        // Detect if the Player collected the fuel
        if (other.CompareTag("Player"))
        {
            // Restore fuel via GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestoreFuel(fuelAmountToRestore);
            }
            else
            {
                Debug.LogWarning("No GameManager instance found!");
            }

            // Disable the collider to avoid collecting it twice in the same frame
            GetComponent<Collider>().enabled = false;

            // Optional: Play a pickup particle effect or sound effect here

            // Destroy the pickup object so it disappears from the track
            Destroy(gameObject);
        }
    }
}
