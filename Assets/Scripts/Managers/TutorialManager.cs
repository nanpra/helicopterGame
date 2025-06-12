using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance { get; private set; }

    [Header("Basic Info")]
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

    bool complete = false;

    void Start()
    {
        instance = this;
        complete = PlayerPrefsExtra.GetBool("isComplete", false);
        if (complete)
        {
            if(SceneManager.GetActiveScene().buildIndex > 0)
            {
                GameManager.Instance.healthSlider.gameObject.SetActive(false);
            }
            else
            {
                GameManager.Instance.healthSlider.gameObject.SetActive(true);
            }
            GameManager.Instance.fuelSlider.gameObject.SetActive(true);
            StartGame();
            return;
        }
            

        Step1();
    }

    void Update()
    {
        if (complete)
            return;

        heliZPos = helicopter.transform.position.z;

        if (currentStep == 1 && JoystickMoved() && heliZPos > 50)
            Step2();

        else if (currentStep == 2 && heliZPos > 85)
            Step3();

        else if (currentStep == 3 && spawnedTurret.transform.position.z + 15 < heliZPos)
            Step4();

        else if (currentStep == 4 && helicopter.transform.position.z > 235)
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
        ArrowMark.transform.rotation = new Quaternion(0,-90,90,0);
        tutInfo.text = "Avoid bullets and lasers from hidden turrets.";
    }

    private void Step4()
    {
        OpenPanel();
        currentStep = 4;
        Vector3 healthPos = new Vector3(helicopter.transform.position.x + Random.Range(-20, 20) , 35 , heliZPos + Random.Range(50, 70));
        Vector3 fuelPos = new Vector3(helicopter.transform.position.x + Random.Range(-20, 20) , 35 , heliZPos + Random.Range(50, 70));
        Instantiate(health, healthPos, Quaternion.identity);
        Instantiate(fuel, fuelPos, Quaternion.identity);
        tutInfo.text = "Collect Fuel and Gear to survive!";
    }
    private void Step5()
    {
        OpenPanel();
        tutInfo.text = "Survive and create high score to collect more coins";
    }
    public void ContinueBtn()
    {
        Time.timeScale = 1;
        tutorialPanel.SetActive(false);
        AudioManager.instance.Play("HeliSound");
    }

    private void OpenPanel()
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0;
        AudioManager.instance.Stop("HeliSound");
    }

    public void StartGame()
    {
        GameManager.Instance.scoreText.rectTransform.DOAnchorPos(Vector2.down * 74, 1);
        tutorialOver = true;
        if (spawnedTurret != null) Destroy(spawnedTurret);
        gameObject.SetActive(false);
    }

    IEnumerator UiDelay()
    {
        yield return new WaitForSecondsRealtime(1);
        Step5();
        yield return new WaitForSecondsRealtime(1.5f);
        StartGame();
        PlayerPrefsExtra.SetBool("isComplete", true);

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
