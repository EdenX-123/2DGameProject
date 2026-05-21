using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    void Start()
    {
        // if the button component exists,
        // add a listener to play the click sound when clicked
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(() => AudioManager.instance.PlayButtonClick());
    }
}
