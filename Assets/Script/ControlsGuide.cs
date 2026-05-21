using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class ControlsGuide : MonoBehaviour
{
    [Header("UI")]
    public GameObject controlsPanel;
    public float autoHideTime = 5f;

    [Header("闪烁提示")]
    public TextMeshProUGUI hintText;

    void Start()
    {
        // 游戏开始时显示
        controlsPanel.SetActive(true);
        Time.timeScale = 0f; // 暂停游戏等玩家看完

        StartCoroutine(AutoHide());
        StartCoroutine(BlinkHint()); // ← 加这行
    }

    IEnumerator AutoHide()
    {
        // 用 unscaledTime 因为 timeScale = 0
        yield return new WaitForSecondsRealtime(autoHideTime);
        Hide();
    }

    IEnumerator BlinkHint()
    {
        while (true)
        {
            hintText.alpha = 1f;
            yield return new WaitForSecondsRealtime(0.6f);
            hintText.alpha = 0f;
            yield return new WaitForSecondsRealtime(0.4f);
        }
    }

    public void Hide()
    {
        StopAllCoroutines();
        controlsPanel.SetActive(false);
        Time.timeScale = 1f; // 恢复游戏
    }

    void Update()
    {
        // 按任意键也可以关闭
        if (controlsPanel.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                Hide();
            }
        }
    }
}
