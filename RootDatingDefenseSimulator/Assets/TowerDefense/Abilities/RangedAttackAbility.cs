using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedAttackAbility : TreeAbility {

    private GameSettings gameSettings;
    private Vector3 projectileSpawnPos;

    private float attackDamage;
    private float attackRange;
    private int layerMask;
    private Enemy target;

    public override void Init(TreeStatblock stats, GameSettings gameSettings) {
        this.gameSettings = gameSettings;
        attackDamage = stats.Attack;
        attackRange = stats.Range;
        layerMask = 1 << 7;
        projectileSpawnPos = transform.position + Vector3.up;
    }

    public void SetProjectileSpawnPoint(Vector3 pos) {
        projectileSpawnPos = pos;
    }

    public override bool TryPerform() {
        if(target != null) {
            //Attack
            Attack();
            return true;
        }

        List<Enemy> enemies = Physics.OverlapSphere(transform.position, attackRange, layerMask)
            .Select(c => c.GetComponent<Enemy>())
            .Where(e => e != null)
            .ToList();

        if(enemies.Count > 0) {
            target = enemies[0];
            Attack();
            return true;
        }

        return false;
    }

    private void Attack() {
        PhotonNetwork.Instantiate("Projectile", projectileSpawnPos, Quaternion.identity).GetComponent<Projectile>().Init(target, attackDamage);
    }
}
