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
    public void SpawnEnemy()
    {
        if (currentEnemy != null) return;
        currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
    public void RespawnEnemy()
    {
        if (currentEnemy != null)
            Destroy(currentEnemy);

        currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        Debug.Log("Enemy respawned!");
    }
}
