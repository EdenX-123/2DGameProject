using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    void Awake()
    {
        Time.timeScale = 1f; // 确保游戏开始时是正常速度
    }
    
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // pause
        isPaused = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // continue
        isPaused = false;
    }

    public void QuitGame()
    {
        SceneTransition.instance.LoadScene("MainMenu");
        Time.timeScale = 1f; // continue
    }
}
