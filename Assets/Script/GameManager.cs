using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<Checkpoint> allCheckpoints = new List<Checkpoint>();

    [Header("respawn settings")]
    public Transform defaultRespawnPoint; // 游戏开始的默认复活点
    private Vector3 currentCheckpointPos;

    [Header("Monster settings")]
    private List<EnemySpawner> allSpawners = new List<EnemySpawner>();
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

        Application.targetFrameRate = 120; // 或 144 / 165
        QualitySettings.vSyncCount = 0;

        // 默认复活点必须在 Awake 设置
        if (defaultRespawnPoint != null)
            currentCheckpointPos = defaultRespawnPoint.position;
        else
            Debug.LogWarning("GameManager: defaultRespawnPoint 没有设置！");
    }

    void Start()
    {
        allSpawners.AddRange(FindObjectsByType<EnemySpawner>(FindObjectsInactive.Exclude));
        allCheckpoints.AddRange(FindObjectsByType<Checkpoint>(FindObjectsInactive.Exclude)); // ✅
    }

    // Checkpoint 激活时调用
    public void SetCheckpoint(Vector3 pos)
    {
        currentCheckpointPos = pos;
        Debug.Log("Checkpoint point updated: " + pos);
    }

    // 掉落用 → 最近激活的存档点
    public Vector3 GetCheckpointPos()
    {
        return currentCheckpointPos;
    }

    public Vector3 GetDefaultRespawnPos()
    {
        return defaultRespawnPoint.position;
    }

    // when player dies, call this to reset game state 
    // (respawn monsters, reset checkpoints, etc.)
    public void PlayerDied()
    {
        // respawn all monsters
        foreach (EnemySpawner spawner in allSpawners)
        {
            spawner.RespawnEnemy();
        }
            //reset all checkpoints
        foreach (Checkpoint cp in allCheckpoints)
            cp.ResetCheckpoint();

        // reset respawn point to default
        currentCheckpointPos = defaultRespawnPoint.position;

        // reset checkpoint manager index for player respawn logic
        if (CheckpointManager.instance != null)
            CheckpointManager.instance.currentIndex = -1;
    }
    
}

// 加在 GameManager.cs 文件底部，类外面
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;
    public int currentIndex = -1; // -1 = 还没激活任何存档点

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
}
