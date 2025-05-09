using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { Playing, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Playing;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public Slider fuelSlider;
    public Slider healthSlider;
    public GameObject gameOverPanel;
    public GameObject gamePlayPanel;

    [Header("Game Settings")]
    public float fuelConsumptionRate = 0.1f;
    public float scoreIncreaseRate = 3f;

    [Header("Refrences")]
    public Helicopter helicopterScript;
    public ProceduralGeneration proceduralGenerationScript;
    public PoolingObjects poolingObjectsScript;

    private float score = 0f;
    private bool isGameOver = false;
    public bool gasOver;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            UpdateScore();
            DecreaseFuel();
        }

        if (fuelSlider.value <= 0 && !isGameOver  || healthSlider.value <= 0)
        {
            GameOver();
        }
    }

    private void UpdateScore()
    {
        score += scoreIncreaseRate * Time.deltaTime;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }
    }

    private void DecreaseFuel()
    {
        fuelSlider.value -= fuelConsumptionRate * Time.deltaTime;
        if (fuelSlider.value <= 0f)
        {
            gasOver = true;
        }
    }

    public void GameOver()
    {
        helicopterScript.enabled = false;
        isGameOver = true;
        CurrentState = GameState.GameOver;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("Game Over! Final Score: " + Mathf.FloorToInt(score));
    }

    public void RestartGame()
    {
        helicopterScript.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        CurrentState = GameState.Playing;
        isGameOver = false;
        score = 0;
        fuelSlider.value = 1;
        gameOverPanel.SetActive(false);
    }

    public void StartGame()
    {
        Application.LoadLevel(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
