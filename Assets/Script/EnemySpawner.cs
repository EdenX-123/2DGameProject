using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private GameObject currentEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
          SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //when game starts, spawn an enemy at the spawner's position
    public void SpawnEnemy()
    {
        if (currentEnemy != null) return;
        currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
    //when player dies, respawn the enemy at the spawner's position
    public void RespawnEnemy()
    {
        if (currentEnemy != null)
            Destroy(currentEnemy);

        currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        Debug.Log("Enemy respawned!");
    }
}
