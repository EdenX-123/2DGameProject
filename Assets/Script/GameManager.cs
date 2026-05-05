using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

       void Awake()
    {
        // 单例（保证只有一个 GameManager）
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple GameManager instances detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }
}
