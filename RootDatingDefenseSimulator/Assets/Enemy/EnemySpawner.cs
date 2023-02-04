using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    private Enemy enemyPrefab;

    private const float spawnCooldown = 3f;
    private float timeUntilNextSpawn = 0f;

    private void Awake() {
        Assert.IsNotNull(enemyPrefab);
        timeUntilNextSpawn = spawnCooldown;
    }

    private void Update() {
        timeUntilNextSpawn -= Time.deltaTime;
        if(timeUntilNextSpawn <= 0f) {
            timeUntilNextSpawn = spawnCooldown;
            Enemy enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            //enemy.GetComponent<NavMeshAgent>().set
        }
    }
}
