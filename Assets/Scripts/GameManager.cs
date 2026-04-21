using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Speed Settings")]
    [Tooltip("Controls how fast the world moves towards the player.")]
    public float globalWorldSpeed = 10f;

    [Header("Values")]
    public int scoreBilletes = 0;
    public float timeSurvived = 0f;

    [Header("Health Settings")]
    [Tooltip("Max capacity of health sections. Clamped internally.")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Fuel Settings")]
    [Tooltip("Max capacity of fuel sections. Clamped internally.")]
    public float maxFuel = 10f;
    private float currentFuel;
    
    [Tooltip("Base amount of fuel depleted per second.")]
    public float baseFuelDepletionRate = 0.5f;
    [Tooltip("How much the fuel depletion dynamically accelerates per second survived.")]
    public float fuelDepletionAcceleration = 0.015f;

    private bool isGameOver = false;

    private void Awake()
    {
        // Singleton pattern implementation to easily access GameManager from anywhere
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize health and fuel at the start of the game
        currentHealth = maxHealth;
        currentFuel = maxFuel;
        isGameOver = false;
        timeSurvived = 0f;
    }

    private void Update()
    {
        // Stop updating values if the game is over
        if (isGameOver) return;

        // Increase survival time
        timeSurvived += Time.deltaTime;

        // Calculate the continuous fuel depletion rate (accelerating over time)
        float currentDepletionRate = baseFuelDepletionRate + (timeSurvived * fuelDepletionAcceleration);
        
        // Continuously deplete fuel using Time.deltaTime
        currentFuel -= currentDepletionRate * Time.deltaTime;

        // Clamp fuel between 0 and its max capacity (10)
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        // Check if fuel has run out
        if (currentFuel <= 0f)
        {
            currentFuel = 0f;
            HandleGameOver();
        }
    }

    /// <summary>
    /// Reduces the player's health by the specified amount and checks for Game Over.
    /// </summary>
    /// <param name="amount">Amount of damage to take.</param>
    public void TakeDamage(int amount)
    {
        if (isGameOver) return;

        currentHealth -= amount;

        // Ensure health stays clamped between 0 and max
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Check if health has depleted completely
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HandleGameOver();
        }
    }

    /// <summary>
    /// Restores fuel by the specified amount sections/points.
    /// </summary>
    /// <param name="amount">Amount of fuel to restore.</param>
    public void RestoreFuel(int amount)
    {
        if (isGameOver) return;

        currentFuel += amount;

        // Ensure fuel never exceeds its max capacity
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
    }

    /// <summary>
    /// Adds collected currency to the score.
    /// </summary>
    /// <param name="amount">Amount of currency to add.</param>
    public void AddBillete(int amount = 1)
    {
        if (isGameOver) return;
        scoreBilletes += amount;
    }

    /// <summary>
    /// Handles the end of the game when health or fuel reaches 0.
    /// </summary>
    private void HandleGameOver()
    {
        isGameOver = true;
        globalWorldSpeed = 0f; // Stop the world from moving
        Debug.Log("Game Over! Time Survived: " + timeSurvived + "s, Score: " + scoreBilletes);
        
        // TODO: Trigger Game Over UI events, stop player animations, format scores, etc.
    }
}
