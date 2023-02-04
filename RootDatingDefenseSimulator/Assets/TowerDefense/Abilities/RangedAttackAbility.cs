using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedAttackAbility : TreeAbility {
    private float attackDamage;
    private float attackRange;
    private int layerMask;

    public override void Init(Character character) {
        //Gather values from character.

        layerMask = 1 << 7;
    }

    public override void Perform() {
        List<Enemy> enemies = Physics.OverlapSphere(transform.position, attackRange)
            .Select(c => c.GetComponent<Enemy>())
            .Where(e => e != null)
            .ToList();

        if(enemies.Count > 0) {

        }
    }
}
