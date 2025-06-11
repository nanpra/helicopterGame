/*using MoreMountains.NiceVibrations;*/
using DG.Tweening;
using Lofelt.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsButton;
    public GameObject pausePanel;
    public Toggle muteToggle, vibrationToggle;
    public Animator muteAnim, vibrationAnim;



    public void AssignToggles()
    {

        pausePanel.GetComponent<CanvasGroup>().alpha = 0;

        SetToggles(muteToggle, PlayerPrefsExtra.GetBool("MuteToggle", true), muteAnim);
        SetToggles(vibrationToggle, PlayerPrefsExtra.GetBool("VibrationToggle", true), vibrationAnim);
        pausePanel.GetComponent<CanvasGroup>().alpha = 1;

    }

    public void PauseGame()
    {
        if (vibrationToggle.isOn)
        {
            print("vibrate");
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }
        settingsButton.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(true);
        AudioListener.volume = 0;

        AssignToggles();
    }
    public void ResumeGame()
    {
        if (vibrationToggle.isOn)
        {
            print("vibrate");
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }
        pausePanel?.GetComponent<CanvasGroup>().DOFade(0, 0.25f);
        pausePanel.transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos3DY(417f, 0.25f).OnComplete(() =>
        {
            settingsButton.gameObject.SetActive(true);
            pausePanel.gameObject.SetActive(false);
        });
        

    }

    public void OnClickMusicOnOff()
    {
        if(muteToggle.isOn)
        {
            muteAnim.SetBool("isToggleOn", true);
        }
        else
        {
            muteAnim.SetBool("isToggleOn", false);
        }
        PlayerPrefsExtra.SetBool("MuteToggle", muteToggle.isOn);
        AudioListener.volume = muteToggle.isOn ? 1 : 0;
    }

    public void OnClickVibrationOnOff()
    {
        if (vibrationToggle.isOn)
        {
            //MMVibrationManager.SetHapticsActive(false);
            vibrationAnim.SetBool("isToggleOn", true);

        }
        else
        {
            //MMVibrationManager.SetHapticsActive(true);
            vibrationAnim.SetBool("isToggleOn", false);

        }
       
        PlayerPrefsExtra.SetBool("VibrationToggle", vibrationToggle.isOn);
    }

    public void SetToggles(Toggle toggle, bool isOn, Animator anim)
    {
        toggle.isOn = isOn;
        anim.SetBool("isToggleOn", isOn);
        AudioListener.volume = muteToggle.isOn ? 1 : 0;

    }

}
