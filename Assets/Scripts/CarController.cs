using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Maximum lateral speed of the car.")]
    public float lateralSpeed = 15f;
    
    [Header("Visual Settings")]
    [Tooltip("Transform that contains the car's visual model. Used to apply tilt/rotation without affecting the root physics movement.")]
    public Transform carModel;
    [Tooltip("Maximum rotation angle on the Y axis (steering).")]
    public float maxSteerAngle = 15f;
    [Tooltip("Maximum tilt angle on the Z axis (banking).")]
    public float maxTiltAngle = 5f;
    [Tooltip("How fast the car interpolates its rotation.")]
    public float rotationSpeed = 10f;

    [Header("Damage Settings")]
    [Tooltip("Invincibility duration in seconds after taking damage.")]
    public float invincibilityDuration = 1f;

    private Rigidbody rb;
    private float horizontalInput;
    private float lastDamageTime = -999f; // Tracks when the player last took damage to enforce the cooldown

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Ensure the Rigidbody is set up correctly in code for an arcade feel
        rb.useGravity = false; // The car does not jump or fall in this layout, it just moves laterally
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth movement
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Better for fast lateral movements colliding with walls
        
        // Freeze rotation so physics collisions don't spin the car unpredictably
        // Freeze Y and Z positions as our car only moves laterally (X-axis) and the world moves towards it (Z-axis)
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
    }

    private void Update()
    {
        // 1. Gather Input in the Update loop for snappy frame-accurate response
        horizontalInput = 0f;
        
        // We use GetKey instead of Input.GetAxis for instant, rigid arcade feedback (no ramp up/down time)
        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }

        // 2. Handle the visual banking/steering based on our input
        HandleVisualEffects();
    }

    private void FixedUpdate()
    {
        // 3. Apply physics-based movement in FixedUpdate
        HandleMovement();
    }

    private void HandleMovement()
    {
        // By setting velocity directly, we override inertia. 
        // When input is 0, velocity immediately becomes 0, creating a snappy arcade feel.
        Vector3 targetVelocity = new Vector3(horizontalInput * lateralSpeed, 0f, 0f);
        
        // Apply only the X velocity. Y and Z are strictly 0.
        rb.linearVelocity = targetVelocity;
    }

    private void HandleVisualEffects()
    {
        if (carModel == null) return;

        // Calculate target rotation based on input direction
        // Rotate Y for steering (left/right visual turn) and Z for banking (tilting)
        float targetYRotation = horizontalInput * maxSteerAngle;
        float targetZTilt = -horizontalInput * maxTiltAngle; // Negative so it tilts "into" the turn

        Quaternion targetRotation = Quaternion.Euler(0f, targetYRotation, targetZTilt);

        // Smoothly interpolate current rotation to the target rotation using Lerp
        carModel.localRotation = Quaternion.Lerp(carModel.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerStay(Collider other)
    {
        // Detect continuous contact with damage zones
        if (other.CompareTag("DamageBorder"))
        {
            // Enforce invincibility cooldown timeframe so health doesn't drain instantly
            if (Time.time >= lastDamageTime + invincibilityDuration)
            {
                TakeDamage();
            }
        }
    }

    private void TakeDamage()
    {
        lastDamageTime = Time.time;
        
        // Safely call the GameManager Singleton to take damage
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReduceHealth(1);
            Debug.Log("Car hit damage border! 1 Health reduced.");
        }
        else
        {
            Debug.LogWarning("GameManager instance not found in scene!");
        }
    }
}
