using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    void Start()
    {
        // 自动给按钮加点击音效
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(() => AudioManager.instance.PlayButtonClick());
    }
}
