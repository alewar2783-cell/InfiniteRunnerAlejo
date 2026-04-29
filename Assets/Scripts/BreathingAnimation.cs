using UnityEngine;

public class BreathingAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How fast the breathing animation plays.")]
    public float baseAnimationSpeed = 3f;
    
    [Tooltip("How dramatic the squash and stretch effect is. Keep this small (e.g., 0.05 to 0.15) for subtle breathing.")]
    public float animationAmplitude = 0.1f;
    
    [Tooltip("Random offset to prevent all obstacles from breathing in perfect robotic synchronization.")]
    public float randomAnimationOffset = 0f;

    // We must remember the object's original scale to apply mathematics to it,
    // rather than accumulating size changes frame over frame which would distort the object.
    private Vector3 originalScale;

    private void Start()
    {
        // Store the exact starting scale of this 3D model
        originalScale = transform.localScale;

        // Automatically assign a random offset between 0 and 100 if the user left it at 0,
        // ensuring every instance of this prefab naturally breathes out of sync with the others.
        if (randomAnimationOffset == 0f)
        {
            randomAnimationOffset = Random.Range(0f, 100f);
        }
    }

    private void Update()
    {
        // 1. Calculate the Sine Wave
        // Mathf.Sin outputs a value smoothly oscillating between -1 and 1 over time.
        float sinWave = Mathf.Sin((Time.time * baseAnimationSpeed) + randomAnimationOffset);

        // 2. Calculate the Squash and Stretch factors
        // When the wave is positive, 'stretchFactor' grows while 'squashFactor' shrinks.
        // When the wave is negative, they reverse. This maintains the illusion of constant volume!
        float stretchFactor = 1f + (sinWave * animationAmplitude);
        float squashFactor = 1f - (sinWave * animationAmplitude);

        // 3. Apply the scale
        // Y-Axis stretches (breathing in/chest rising).
        // X and Z-Axes squash (chest narrowing) to compensate for the volume shift.
        transform.localScale = new Vector3(
            originalScale.x * squashFactor, 
            originalScale.y * stretchFactor, 
            originalScale.z * squashFactor
        );
    }
}
