using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Score Interface")]
    [Tooltip("Drag your TextMeshPro text here.")]
    public TextMeshProUGUI scoreText;

    [Header("Status Bars")]
    [Tooltip("Drag your Health UI Slider here.")]
    public Slider healthBar;
    [Tooltip("Drag your Fuel UI Slider here.")]
    public Slider fuelBar;

    [Header("Game Over Interface")]
    [Tooltip("Drag the entire Game Over UI Panel from your hierarchy here.")]
    public GameObject gameOverPanel;

    private void Awake()
    {
        // Singleton Implementation 
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Make perfectly sure the game over panel isn't sticking out when you click play
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Displays formatted scores avoiding messy decimals via formatting ("F0").
    /// </summary>
    public void UpdateScore(float score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("F0");
        }
    }

    /// <summary>
    /// Refreshes the Health UI elements
    /// </summary>
    public void UpdateHealthBar(int current, int max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = current;
        }
    }

    /// <summary>
    /// Refreshes the Fuel UI elements
    /// </summary>
    public void UpdateFuelBar(float current, float max)
    {
        if (fuelBar != null)
        {
            fuelBar.maxValue = max;
            fuelBar.value = current;
        }
    }

    /// <summary>
    /// Un-hides the Game Over UI Screen.
    /// </summary>
    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Reloads the exact scene the user is currently playing to seamlessly start over.
    /// Wire this directly via the OnClick() handler of your Buttons.
    /// </summary>
    public void RestartGame()
    {
        // Extremely critical: A TimeScale of 0 pauses everything instantly.
        // It persists across scenes, so we MUST return it to 1 BEFORE we load.
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Disconnects from the app. Wire to a 'Quit' button's OnClick()
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Application Attempting Exit...");
        Application.Quit();
    }
}
