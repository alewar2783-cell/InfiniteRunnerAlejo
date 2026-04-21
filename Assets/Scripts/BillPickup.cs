using UnityEngine;

public class BillPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Detect player
        if (other.CompareTag("Player"))
        {
            // Trigger GameManager sequence safely
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddBill();
            }

            // Optional: You can play an arcade money sound or a sparkle particle visual here.

            // Safely cut out overlapping physics loops to avoid registering 2 points by an accident
            GetComponent<Collider>().enabled = false;

            // Remove the bill from existence once scored
            Destroy(gameObject);
        }
    }
}
