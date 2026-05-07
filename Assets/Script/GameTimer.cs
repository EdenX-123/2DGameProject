using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer instance;

    [Header("UI")]
    public TextMeshProUGUI timerText; // 拖入 UI Text

    private float elapsedTime = 0f;
    private bool isRunning = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateDisplay();
    }

     void UpdateDisplay()
    {
        if (timerText == null) return;

        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 100) % 100);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
        public void StartTimer() 
    { 
        elapsedTime = 0f;
        isRunning = true;
        Debug.Log("Timer started!");
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("Timer stopped! Time: " + GetFormattedTime());
    }

    public void PauseTimer() => isRunning = false;
    public void ResumeTimer() => isRunning = true;

    public string GetFormattedTime()
    {
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 100) % 100);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public float GetElapsedTime() => elapsedTime;
}
