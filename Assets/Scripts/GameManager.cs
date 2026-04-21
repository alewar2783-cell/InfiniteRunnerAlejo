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
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Fuel Settings")]
    public int maxFuel = 10;
    private int currentFuel;
    [Tooltip("Time in seconds before losing 1 fuel section.")]
    public float fuelDepletionRate = 5f;
    private float fuelTimer = 0f;

    private bool isGameOver = false;

    private void Awake()
    {
        // Singleton pattern implementation
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
        fuelTimer = 0f;
        timeSurvived = 0f;
    }

    private void Update()
    {
        // Stop updating values if the game is over
        if (isGameOver) return;

        // Increase survival time
        timeSurvived += Time.deltaTime;

        // Handle fuel depletion over time
        fuelTimer += Time.deltaTime;
        if (fuelTimer >= fuelDepletionRate)
        {
            fuelTimer = 0f;
            currentFuel--;

            // Check if fuel has run out
            if (currentFuel <= 0)
            {
                currentFuel = 0;
                HandleGameOver();
            }
        }
    }

    /// <summary>
    /// Reduces the player's health by the specified amount.
    /// </summary>
    /// <param name="amount">Amount of damage to take.</param>
    public void ReduceHealth(int amount = 1)
    {
        if (isGameOver) return;

        currentHealth -= amount;

        // Check if health has depleted completely
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HandleGameOver();
        }
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
        
        // TODO: Show Game Over UI, Stop player logic, Show replay button, etc.
    }
}
