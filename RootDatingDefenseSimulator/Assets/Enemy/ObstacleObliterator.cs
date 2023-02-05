using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObliterator : EnemyObstacleIgnoring {

    #region Static destructable obstacle management
    private static readonly HashSet<Health> destructableObjects = new();

    public static void RegisterDestructableObstacle(Health health) {
        destructableObjects.Add(health);
    }

    public static void DeregisterDestructableObstacle(Health health) {
        destructableObjects.Remove(health);
    }
    #endregion

    protected override void Move(float distanceToTarget, float deltaTime) {
        base.Move(distanceToTarget, deltaTime);

        List<Health> obstaclesToDestroy = new();
        foreach(Health obstacle in destructableObjects) {
            Vector3 obstaclePosition = GetClosestPoint(obstacle.transform);
            Vector3 toObstacle = obstaclePosition - transform.position;
            toObstacle.y = 0f;

            if(toObstacle.sqrMagnitude < reachedThreshhold * reachedThreshhold) {
                obstaclesToDestroy.Add(obstacle);
            }
        }

        while(obstaclesToDestroy.Count > 0) {
            int lastIndex = obstaclesToDestroy.Count - 1;
            Health obstacle = obstaclesToDestroy[lastIndex];
            obstacle.Damage(attackDamage);
            PhotonNetwork.Instantiate("Explosion", obstacle.transform.position, Quaternion.identity);
            obstaclesToDestroy.RemoveAt(lastIndex);
        }
    }
}
