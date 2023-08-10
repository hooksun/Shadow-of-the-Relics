using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingMenu;

    void Start()
    {
        settingMenu.SetActive(false);
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

    public void Credit(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}
