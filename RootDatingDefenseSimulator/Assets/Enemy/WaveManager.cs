using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class WaveManager : MonoBehaviour {

    [SerializeField]
    private GameSettings gameSettings;

    [SerializeField]
    private WaveSettings waveSettings;

    [SerializeField]
    private EnemyVariants enemyVariants;

    [SerializeField]
    private List<EnemySpawnPoint> enemySpawnPoints;

    public Action<int> OnNewWave;

    private bool isInitalized = false;
    private int waveIndex = -1;
    private float timeUntilNextWave;
    private System.Random random = new();
    private List<EnemyVariant> enemiesLeftToSpawn = new();

    public void Init() {
        Assert.IsNotNull(gameSettings);
        Assert.IsNotNull(waveSettings);
        Assert.IsNotNull(enemyVariants);
        Assert.IsTrue(enemySpawnPoints.Count > 0);
        timeUntilNextWave = gameSettings.timeBetweenWaves;
        isInitalized = true;
    }

    private void Update() {
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER || !isInitalized) {
            return;
        }

        if(enemiesLeftToSpawn.Count > 0) {
            // Wait
        }
        else if(timeUntilNextWave <= 0) {

            timeUntilNextWave = gameSettings.timeBetweenWaves;
            ++waveIndex;
            OnNewWave?.Invoke(waveIndex);
            StartCoroutine(SpawnWave());
        }
        else {
            timeUntilNextWave -= Time.deltaTime;
        }
    }

    private IEnumerator SpawnWave() {
        Debug.Log($"Started wave {waveIndex}");
        enemiesLeftToSpawn.Clear();
        Wave wave = waveSettings.waves[waveIndex % waveSettings.waves.Count];
        foreach(SpawnGroup group in wave.groups) {
            EnemyVariant enemyVariant = new();
            enemyVariant.id = group.enemyVariant;
            enemyVariant.stats = enemyVariants.GetVariantStats(enemyVariant.id, waveIndex);
            for(int i = 0; i < group.count; ++i) {
                enemiesLeftToSpawn.Add(enemyVariant);
            }
        }
        enemiesLeftToSpawn = enemiesLeftToSpawn.OrderBy(_ => random.Next()).ToList();

        while(enemiesLeftToSpawn.Count > 0) {
            SpawnGroup();
            yield return new WaitForSeconds(gameSettings.timeBetweenEnemySpawns);
        }
    }

    private void SpawnGroup() {
        foreach(EnemySpawnPoint spawnPoint in enemySpawnPoints) {
            if(enemiesLeftToSpawn.Count == 0) {
                return;
            }
            int index = enemiesLeftToSpawn.Count - 1;
            EnemyVariant enemyVariant = enemiesLeftToSpawn[index];
            PhotonNetwork.Instantiate($"Enemy{enemyVariant.id}", spawnPoint.transform.position, Quaternion.identity, 0, enemyVariant.stats.ToObjectArray());
            spawnPoint.OnSpawn();
            enemiesLeftToSpawn.RemoveAt(index);
        }
    }
}
