using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [Header("Panel References")]
    public GameObject mainMenuPanel;
    public GameObject gameplayPanel;
    public GameObject tutorialPanel;

    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI coinsText;
    public int highScore { get; private set; }

    public int coins { get; private set; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GameplayEvents.startFlying.AddListener(StartGameOnTap);
        
        highScore = PlayerPrefs.GetInt("highScore", highScore);
        SetHighScore(highScore);

        coins = PlayerPrefs.GetInt("coins", coins);
        coinsText.SetText(this.coins.ToString());
    }
    public void StartButton()
    {
        GameplayEvents.takeOff?.Invoke();
        mainMenuPanel.SetActive(false);
    }
    private void StartGameOnTap()
    {
        gameplayPanel.SetActive(true);
    }

    public void SetHighScore(int score)
    {
        highScore = score;
        highScoreText.SetText("High Score : " + highScore.ToString());
        PlayerPrefs.SetInt("highScore", highScore);
    }
    public void SetCoins(int coins)
    {
        this.coins += coins;
        coinsText.SetText(this.coins.ToString());
        PlayerPrefs.SetInt("coins", this.coins);
    }

    private void OnDestroy()
    {
        GameplayEvents.takeOff.RemoveListener(StartGameOnTap);
    }
}
