using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Speed & Difficulty Scaling")]
    public float startingWorldSpeed = 10f;
    [Tooltip("Maximum cap the speed can reach.")]
    public float maxWorldSpeed = 35f;
    [Tooltip("How much the world accelerates per every total score point.")]
    public float speedMultiplierPerScore = 0.005f;
    public float globalWorldSpeed { get; private set; }

    [Header("Score Settings")]
    public int billsCollected = 0;
    public float scorePerBill = 50f;
    public float totalScore = 0f;
    public float timeSurvived = 0f;

    [Header("Health Settings")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Fuel Settings")]
    public float maxFuel = 10f;
    private float currentFuel;
    public float baseFuelDepletionRate = 0.5f;
    public float maxFuelDepletionRate = 2.5f;
    [Tooltip("How much fuel depletion accelerates per score point.")]
    public float fuelDepletionAcceleration = 0.0005f;

    private bool isGameOver = false;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Initialize logic on restart/play
        Time.timeScale = 1f; // Must ensure time is moving if we restart the game!
        currentHealth = maxHealth;
        currentFuel = maxFuel;
        isGameOver = false;
        timeSurvived = 0f;
        billsCollected = 0;
        globalWorldSpeed = startingWorldSpeed;
        
        // Push initial UI values to the screen immediately
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
            UIManager.Instance.UpdateFuelBar(currentFuel, maxFuel);
            UIManager.Instance.UpdateScore(0f);
        }
    }

    private void Update()
    {
        if (isGameOver) return;

        // 1. Base Timers & Score Math
        timeSurvived += Time.deltaTime;
        // Dynamically compute score: standard endurance points + collectible points
        totalScore = (timeSurvived * 10f) + (billsCollected * scorePerBill);

        // 2. Difficulty Scaling calculations
        globalWorldSpeed = startingWorldSpeed + (totalScore * speedMultiplierPerScore);
        globalWorldSpeed = Mathf.Clamp(globalWorldSpeed, startingWorldSpeed, maxWorldSpeed);

        float currentDepletionRate = baseFuelDepletionRate + (totalScore * fuelDepletionAcceleration);
        currentDepletionRate = Mathf.Clamp(currentDepletionRate, baseFuelDepletionRate, maxFuelDepletionRate);
        
        // 3. Apply Continuous Fuel Depletion
        currentFuel -= currentDepletionRate * Time.deltaTime;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        // 4. Update real-time UI components
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(totalScore);
            UIManager.Instance.UpdateFuelBar(currentFuel, maxFuel);
        }

        // 5. Game Over triggers
        if (currentFuel <= 0f)
        {
            currentFuel = 0f;
            HandleGameOver();
        }
    }

    /// <summary>
    /// Adds 1 to the multiplier score when a bill is picked up.
    /// </summary>
    public void AddBill()
    {
        if (isGameOver) return;
        billsCollected++;
    }

    public void TakeDamage(int amount)
    {
        if (isGameOver) return;
        
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Update Health UI on impact
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HandleGameOver();
        }
    }

    public void RestoreFuel(int amount)
    {
        if (isGameOver) return;
        
        currentFuel += amount;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        
        // Force the visual UI update just to be perfectly snappy
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateFuelBar(currentFuel, maxFuel);
        }
    }

    private void HandleGameOver()
    {
        isGameOver = true;
        globalWorldSpeed = 0f; 
        
        // Freeze all physical time & physics using TimeScale
        Time.timeScale = 0f;
        
        // Fire instructions over to the UI manager to pop the screen open
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel();
        }
    }
}
