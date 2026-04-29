using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Controls how fast the wheel spins relative to the world's movement speed. Tweak this until the tires look like they are gripping the road natively.")]
    public float rotationMultiplier = 100f;
    
    [Tooltip("The local axis the wheel should spin around. (Typically X for standard wheel alignment, but might be Y or Z depending on your 3D model import settings).")]
    public Vector3 rotationAxis = Vector3.right; // Vector3.right is (1, 0, 0) - the X axis

    private void Update()
    {
        // Safety check to ensure the GameManager exists
        if (GameManager.Instance != null && GameManager.Instance.globalWorldSpeed > 0f)
        {
            // Calculate the total rotation step for this exact frame
            float rotationStep = GameManager.Instance.globalWorldSpeed * rotationMultiplier * Time.deltaTime;

            // Apply rotation locally so the wheel spins on its own axis, regardless of how the car turns/tilts
            transform.Rotate(rotationAxis * rotationStep, Space.Self);
        }
    }
}
