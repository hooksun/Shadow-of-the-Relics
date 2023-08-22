using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingMenu, quitButton;
    public Button ContinueButton;

    void Awake()
    {
        if(!SaveManager.hasSaveFile)
            ContinueButton.interactable = false;

        #if UNITY_WEBGL
        quitButton.SetActive(false);
        #endif
    }

    public void NewGame(int sceneID)
    {
        SaveManager.loadOnStart = false;
        SceneManager.LoadScene(sceneID);
    }

    public void Continue(int sceneID)
    {
        SaveManager.loadOnStart = true;
        SceneManager.LoadScene(sceneID);
    }

    public void OpenSetting()
    {
        settingMenu.SetActive(true);
    }

    public void CloseSetting()
    {
        settingMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
