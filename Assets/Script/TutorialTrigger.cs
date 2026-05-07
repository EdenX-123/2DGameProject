using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialTrigger : MonoBehaviour
{
    [Header("提示设置")]
    [TextArea] public string tutorialText;      // 提示内容
    public float displayTime = 3f;              // 显示时间
    public float fadeSpeed = 2f;               // 淡入淡出速度

    [Header("UI 引用")]
    public GameObject tutorialPanel;
    public Text messageText;                   // 或用 TMPro

    private bool hasTriggered = false;
    private static TutorialTrigger activeTrigger; // 防止多个提示同时显示

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            // 如果有其他提示在显示，先停掉
            if (activeTrigger != null && activeTrigger != this)
                activeTrigger.StopAllCoroutines();

            activeTrigger = this;
            StartCoroutine(ShowTutorial());
        }
    }

    IEnumerator ShowTutorial()
    {
        // 显示面板
        tutorialPanel.SetActive(true);
        messageText.text = tutorialText;

        // 淡入
        CanvasGroup cg = tutorialPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            while (cg.alpha < 1f)
            {
                cg.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        yield return new WaitForSeconds(displayTime);

        // 淡出
        if (cg != null)
        {
            while (cg.alpha > 0f)
            {
                cg.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        tutorialPanel.SetActive(false);
        activeTrigger = null;
    }

    public void StopTutorial()
    {
        StopAllCoroutines();
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }
}
