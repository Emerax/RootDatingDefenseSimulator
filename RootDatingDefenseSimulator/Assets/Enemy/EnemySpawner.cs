using Photon.Pun;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    private const float spawnCooldown = 3f;

    private float timeUntilNextSpawn = 3f;

    private void Update() {
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER) {
            return;
        }

        if(timeUntilNextSpawn > 0f) {
            timeUntilNextSpawn -= Time.deltaTime;
        }
        else if(Enemy.HasPotentialTargets()) {
            timeUntilNextSpawn = spawnCooldown;
            PhotonNetwork.Instantiate("Enemy", transform.position, Quaternion.identity);
        }
    }
}
