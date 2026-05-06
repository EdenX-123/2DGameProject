using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    public bool isActivated = false;
    [SerializeField] public int checkpointIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isActivated) return;

        // 只激活比当前存档点 index 更大的
        CheckpointManager cm = CheckpointManager.instance;
        if (cm != null && checkpointIndex <= cm.currentIndex) return;

        isActivated = true;
        if (cm != null) cm.currentIndex = checkpointIndex;

        // anim?.SetTrigger("activate");
        Vector3 respawnPos = transform.position + new Vector3(0f, 0f, 0f);
        GameManager.instance.SetCheckpoint(respawnPos);
        Debug.Log("Checkpoint " + checkpointIndex + " activated!");
    }

    public void ResetCheckpoint()
    {
        isActivated = false;
        // anim?.SetTrigger("deactivate"); // 如果有动画可以加
        Debug.Log("Checkpoint " + checkpointIndex + " reset!");
    }
    
}
