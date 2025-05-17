using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [Header("Panel References")]
    public GameObject mainMenuPanel;
    public GameObject gameplayPanel;


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
    private void OnDestroy()
    {
        GameplayEvents.takeOff.RemoveListener(StartGameOnTap);
    }
}
