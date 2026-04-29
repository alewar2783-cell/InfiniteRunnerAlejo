using UnityEngine;

public class CollectibleAnimator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("How fast the collectible spins around its vertical axis (degrees per second).")]
    public float rotationSpeed = 90f;

    [Header("Floating Settings")]
    [Tooltip("How fast the collectible bobs up and down.")]
    public float floatSpeed = 2f;
    [Tooltip("How high and low the collectible travels from its starting point.")]
    public float floatAmplitude = 0.5f;

    [Header("Squash & Stretch Settings")]
    [Tooltip("How fast the collectible breathes/pulses.")]
    public float breathingSpeed = 4f;
    [Tooltip("How dramatic the squash and stretch effect is. Keep under 0.2 for best results.")]
    public float breathingAmplitude = 0.1f;

    // Cache original transform data so we don't permanently distort the object over time
    private Vector3 originalLocalPosition;
    private Vector3 originalScale;
    
    // A random offset so that multiple spawned collectibles don't animate in robotic unison
    private float randomOffset;

    private void Start()
    {
        // Store exact starting vectors
        originalLocalPosition = transform.localPosition;
        originalScale = transform.localScale;

        // Generate a random time offset between 0 and 100 seconds
        randomOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        // A) Continuous Rotation
        // Rotate continuously on the Y-axis (Vector3.up). 
        // Using Time.deltaTime keeps the rotation smooth and frame-rate independent.
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);

        // B) Floating (Hovering)
        // Calculate the current sine wave value for floating based on time + random offset
        float floatSinWave = Mathf.Sin((Time.time * floatSpeed) + randomOffset);
        
        // We modify ONLY the Y axis of the local position. 
        // Using localPosition ensures it moves up and down properly even while the parent Chunk is rocketing backwards!
        Vector3 newPosition = originalLocalPosition;
        newPosition.y += floatSinWave * floatAmplitude;
        transform.localPosition = newPosition;

        // C) Procedural Squash & Stretch (Breathing)
        // Calculate the current sine wave value for breathing (can run at a different speed than floating)
        float breatheSinWave = Mathf.Sin((Time.time * breathingSpeed) + randomOffset);

        // Calculate factors. When Y stretches (positive), X/Z squash (negative) to maintain visual volume.
        float stretchFactor = 1f + (breatheSinWave * breathingAmplitude);
        float squashFactor = 1f - (breatheSinWave * breathingAmplitude);

        // Apply the scale safely using the original proportions
        transform.localScale = new Vector3(
            originalScale.x * squashFactor, 
            originalScale.y * stretchFactor, 
            originalScale.z * squashFactor
        );
    }
}
