using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    private GameSettings gameSettings;

    private float timeUntilNextSpawn = 1f;

    private void Awake() {
        Assert.IsNotNull(gameSettings);
    }

    private void Update() {
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER) {
            return;
        }

        if(timeUntilNextSpawn > 0f) {
            timeUntilNextSpawn -= Time.deltaTime;
        }
        else if(Enemy.HasPotentialTargets()) {
            timeUntilNextSpawn = gameSettings.timeBetweenEnemySpawns;
            PhotonNetwork.Instantiate("Enemy", transform.position, Quaternion.identity);
        }
    }
}
