using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance { get; private set; }

    public TextMeshProUGUI tutInfo;
    public GameObject tutorialPanel;
    public GameObject turret;
    public GameObject fuel;
    public GameObject health;

    private GameObject spawnedTurret;
    [SerializeField]private int currentStep = 0;
    private float heliZPos;
    public bool tutorialOver = false;
    public GameObject helicopter;
    public GameObject arrowMark;
    public OutlineBlinkEffect blinkEffect;

    void Start()
    {
        instance = this;
        Step1();
    }

    void Update()
    {
        heliZPos = helicopter.transform.position.z;

        if (currentStep == 1 && JoystickMoved() && heliZPos > 50)
            Step2();

        else if (currentStep == 2 && heliZPos > 85)
            Step3();

        else if (currentStep == 3 && spawnedTurret.transform.position.z + 15 < heliZPos)
            Step4();

        else if (currentStep == 4 && GameManager.Instance.helicopterScript.lastText)
            StartCoroutine(UiDelay());
    }

    private bool JoystickMoved()
    {
        var input = GameManager.Instance.helicopterScript.joystick.GetJoystickInput();
        return input.x != 0 || input.y != 0;
    }

    private void Step1()
    {
        OpenPanel();
        currentStep = 1;
        tutInfo.text = "Use the joystick to move the helicopter.";
    }

    private void Step2()
    {
        currentStep = 2;
        tutorialPanel.SetActive(true);
        GameManager.Instance.healthSlider.gameObject.SetActive(true);
        GameManager.Instance.healthSlider.gameObject.GetComponentInChildren<OutlineBlinkEffect>().StartBlinking();
        GameManager.Instance.fuelSlider.gameObject.SetActive(true);
        GameManager.Instance.fuelSlider.gameObject.GetComponentInChildren<OutlineBlinkEffect>().StartBlinking();
        tutInfo.text = "Watch your health and fuel bar indicators.";
        StartCoroutine(StopBlinkingEffect());
    }

    private void Step3()
    {
        OpenPanel();
        currentStep = 3;
        Vector3 spawnPos = new Vector3(4, 23, heliZPos + 65);
        spawnedTurret = Instantiate(turret, spawnPos, Quaternion.identity);
        Vector3 arrowPos = spawnedTurret.transform.position + Vector3.up * 4;
        GameObject ArrowMark = Instantiate(arrowMark, arrowPos, Quaternion.identity);
        ArrowMark.transform.rotation = new Quaternion(90,0,0,0);
        tutInfo.text = "Avoid bullets and lasers from hidden turrets.";
    }

    private void Step4()
    {
        OpenPanel();
        currentStep = 4;
        Vector3 healthPos = new Vector3(helicopter.transform.position.x + Random.Range(-20, 20) , 35 , heliZPos + Random.Range(50, 70));
        Vector3 fuelPos = new Vector3(helicopter.transform.position.x + Random.Range(-20, 20) , 35 , heliZPos + Random.Range(50, 70));
        Instantiate(health, healthPos, Quaternion.identity);
        Instantiate(fuel, fuelPos + Vector3.one , Quaternion.identity);
        tutInfo.text = "Collect Fuel and Gear to survive!";
    }
    private void Step5()
    {
        OpenPanel();
        tutInfo.text = "Survive and create high score";
        GameManager.Instance.helicopterScript.lastText = false;
    }
    public void ContinueBtn()
    {
        Time.timeScale = 1;
        tutorialPanel.SetActive(false);
    }

    private void OpenPanel()
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        tutorialOver = true;
        if (spawnedTurret != null) Destroy(spawnedTurret);
        gameObject.SetActive(false);
    }

    IEnumerator UiDelay()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        Step5();
        yield return new WaitForSecondsRealtime(3);
        StartGame();
    }

    IEnumerator TxtDelay()
    {
        yield return new WaitForSecondsRealtime(3);
        tutInfo.text = "Drag up to move upwards";
        tutInfo.text = "Drag down to move downwards";
    }

    IEnumerator StopBlinkingEffect()
    {
        yield return new WaitForSecondsRealtime(2);
        GameManager.Instance.healthSlider.gameObject.GetComponentInChildren<OutlineBlinkEffect>().StopBlinking();
        GameManager.Instance.fuelSlider.gameObject.GetComponentInChildren<OutlineBlinkEffect>().StopBlinking();
    }
}
