using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;
    public GameObject settingMenu;
    public static bool isPaused;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {   
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        AudioPlayer.PauseAll(true);
        BackgroundMusic.PauseMusic(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        settingMenu.SetActive(false);
        Time.timeScale = 1f;
        AudioPlayer.PauseAll(false);
        BackgroundMusic.PauseMusic(false);
        isPaused = false;
        SettingManager.OnSave();
    }

    public void Home(int sceneID)
    {
        Time.timeScale = 1f;
        SaveManager.Save();
        SceneManager.LoadScene(sceneID);
    }

    public void OpenSetting()
    {
        pauseMenu.SetActive(false);
        settingMenu.SetActive(true);
    }

    public void CloseSetting()
    {
        settingMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
