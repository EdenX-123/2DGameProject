using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;

    [SerializeField] private Image fadeImage; // 黑色Image
    [SerializeField] private float fadeDuration = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ✅ 切场景不销毁
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 游戏启动时淡入（黑->透明）
        StartCoroutine(FadeIn());
    }

    // 外部调用：切换场景
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    // 黑屏 → 透明
    IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = 1f - (timer / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;
        fadeImage.gameObject.SetActive(false);
    }

    // 透明 → 黑屏 → 切场景 → 淡入
    IEnumerator FadeAndLoad(string sceneName)
    {
        // 淡出（透明→黑）
        fadeImage.gameObject.SetActive(true);
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = timer / fadeDuration;
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;

        // 切场景
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(sceneName);

        // 等一帧让场景加载完
        yield return null;

        // 淡入（黑→透明）
        StartCoroutine(FadeIn());
    }
}
