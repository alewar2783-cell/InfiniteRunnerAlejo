using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Lighting References")]
    [Tooltip("Drag the main Directional Light (Sun) from your hierarchy into this slot.")]
    public Light sunLight;

    [Header("Time Settings")]
    [Tooltip("How long (in real-world seconds) it takes to complete one full 24-hour cycle (360 degrees).")]
    public float dayDurationInSeconds = 120f;

    private void Update()
    {
        // Safety check to ensure the light is assigned
        if (sunLight != null)
        {
            // Calculate how many degrees the sun needs to rotate this specific frame.
            // 360 degrees divided by the total duration gives degrees per second.
            // Multiplying by Time.deltaTime makes it smooth and independent of frame rate.
            float rotationStep = (360f / dayDurationInSeconds) * Time.deltaTime;

            // Rotate the directional light along the X-axis to simulate the sun rising and setting.
            // Space.Self ensures it rotates relative to its own orientation.
            sunLight.transform.Rotate(Vector3.right * rotationStep, Space.Self);
        }
        else
        {
            Debug.LogWarning("DayNightCycle is missing a reference to the Sun Light!");
        }
    }
}
