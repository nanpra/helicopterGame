using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum GameState { Playing, GameOver, Idle }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState;

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

    [Header("GameStart Refs")]
    public PlayableDirector takeOffTimeline;
    public ProceduralGeneration worldGenerator;
    public CinemachineCamera mainTPPCam;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        GameplayEvents.takeOff.AddListener(StartGameOnTap);
    }
    public void StartGameOnTap()
    {
        StartCoroutine(TakeOff());
    }
    private IEnumerator TakeOff()
    {
        takeOffTimeline.Play();
        mainTPPCam.Priority = 5;
        helicopterScript.propellerAnim.SetBool("isTakingOff", true);
        yield return new WaitUntil(() => takeOffTimeline.state == PlayState.Paused);
        StartFlying();
    }
    private void StartFlying()
    {
        GameplayEvents.startFlying?.Invoke();
        helicopterScript.propellerAnim.SetBool("isFlying", true);

        CurrentState = GameState.Playing;
        helicopterScript.propellerAnim.speed = 1;
        worldGenerator.enabled = true;
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
        /*helicopterScript.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        CurrentState = GameState.Playing;
        isGameOver = false;
        score = 0;
        fuelSlider.value = 1;
        gameOverPanel.SetActive(false);*/
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame()
    {
        Application.LoadLevel(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnDestroy()
    {
        GameplayEvents.takeOff.RemoveListener(StartGameOnTap);
    }
}
