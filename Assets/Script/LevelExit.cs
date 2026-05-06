using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private string MainMenu; // Inspector填下一关名字

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {   
            
            SceneTransition.instance.LoadScene(MainMenu);
        }
    }
}
