using UnityEngine;
using TMPro;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private string MainMenu; // Inspector填下一关名字
    [Header("完成画面 UI")]
    public GameObject completePanel;
    public TextMeshProUGUI finalTimeText;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameTimer.instance.StopTimer();

            // 显示完成画面
            completePanel.SetActive(true);
            finalTimeText.text = "Final Time: " + GameTimer.instance.GetFormattedTime();

            // 暂停游戏
            Time.timeScale = 0f;
        }
    }
}
