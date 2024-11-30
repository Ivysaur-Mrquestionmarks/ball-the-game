using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject MenuCurrent,  MenuNext;
    public Toggle vsyncTog, fullscreen;
    public Dropdown QualityD, ResolutionD;

    public void Start()
    {
        vsyncTog.isOn = (QualitySettings.vSyncCount != 0);
        fullscreen.isOn = Screen.fullScreen;
        QualityD.value = QualitySettings.GetQualityLevel();
    }

    public void leave() {
        Application.Quit();
    }

    public void fullscreenF() {
        Screen.fullScreen = fullscreen.isOn;
    }


    public void vsync()
    {
        
        QualitySettings.vSyncCount = (vsyncTog.isOn ? 1 : 0);
    }

    public void changeMenu()
    {
        MenuCurrent.SetActive(false);
        MenuNext.SetActive(true);
    }

    public void qualityLevel() {
        QualitySettings.SetQualityLevel(QualityD.value, true);
    }
}
