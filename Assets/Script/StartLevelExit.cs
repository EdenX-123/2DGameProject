using UnityEngine;

public class StartLevelExit : MonoBehaviour
{
    [SerializeField] private string MainGame; // Inspector填下一关名字

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {   
            
            SceneTransition.instance.LoadScene(MainGame);
        }
    }
}
