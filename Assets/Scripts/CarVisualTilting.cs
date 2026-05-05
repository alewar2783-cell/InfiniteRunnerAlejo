using UnityEngine;

public class CarVisualTilting : MonoBehaviour
{
    [Header("Tilt & Turn Settings")]
    [Tooltip("How much the car leans into the turn (rolling on the Z-axis).")]
    public float maxLeanAngleZ = 15f;
    
    [Tooltip("How much the nose of the car points left or right (yawing on the Y-axis).")]
    public float maxTurnAngleY = 20f;
    
    [Tooltip("How quickly the car transitions to the target angle and snaps back to center.")]
    public float tiltSpeed = 8f;

    private void Update()
    {
        // 1. Read horizontal input smoothly (-1 for full left, 1 for full right, 0 for no input)
        // Using GetAxis (instead of GetAxisRaw) provides a naturally smooth ramp up/down, 
        // which helps the visual leaning feel less jerky.
        float horizontalInput = Input.GetAxis("Horizontal");

        // 2. Calculate the target angles based on current input
        // Steering Right (+ input) means the Y rotation should be positive.
        float targetTurnY = horizontalInput * maxTurnAngleY;
        
        // Leaning into a Right turn (+ input) means the Z rotation should be negative in Unity.
        // (If you want the car to lean outward like a heavy truck, remove the negative sign!)
        float targetLeanZ = -horizontalInput * maxLeanAngleZ;

        // 3. Create the target rotation state
        // We leave the X-axis at 0 so the car doesn't pitch up or down into the ground.
        Quaternion targetRotation = Quaternion.Euler(0f, targetTurnY, targetLeanZ);

        // 4. Smoothly blend (Lerp) from the current rotation to the target rotation.
        // localRotation is used instead of rotation so this works flawlessly regardless of 
        // the parent object's actual physical orientation in the world.
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, tiltSpeed * Time.deltaTime);
    }
}
