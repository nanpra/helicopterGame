using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject step1;         // "Use joystick to move"
    public GameObject step2;         // "Collect fuel to keep flying"
    public GameObject step3;         // "Avoid turrets or you'll be shot!"
    public Button startButton;       // Start button to begin the game
    public Joystick joystick;        // Reference to the joystick

    private bool joystickUsed = false;
    private bool fuelCollected = false;
    private bool turretAvoided = false;

    void Start()
    {
        ShowStep(1);
    }

    void Update()
    {
        if (!joystickUsed && (GameManager.Instance.helicopterScript.joystick.GetJoystickInput().x != 0 || GameManager.Instance.helicopterScript.joystick.GetJoystickInput().y != 0))
        {
            joystickUsed = true;
            ShowStep(2);
        }

        // You need to set "fuelCollected" and "turretAvoided" externally
    }

    public void SetFuelCollected()
    {
        if (!fuelCollected)
        {
            fuelCollected = true;
            ShowStep(3);
        }
    }

    public void SetTurretAvoided()
    {
        if (!turretAvoided)
        {
            turretAvoided = true;
            startButton.gameObject.SetActive(true);
        }
    }

    private void ShowStep(int step)
    {
        step1.SetActive(step == 1);
        step2.SetActive(step == 2);
        step3.SetActive(step == 3);
    }

    public void StartGame()
    {
        GameManager.Instance.helicopterScript.enabled = true;
        GameManager.Instance.proceduralGenerationScript.enabled = true;
        GameManager.Instance.poolingObjectsScript.enabled = true;
        gameObject.SetActive(false);
    }
}