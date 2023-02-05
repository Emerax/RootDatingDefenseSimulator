using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyObstacleAvoiding : Enemy {

    #region Static destructable obstacle management
    private static readonly HashSet<Health> destructableObjects = new();

    public static void RegisterDestructableObstacle(Health health) {
        destructableObjects.Add(health);
    }

    public static void DeregisterDestructableObstacle(Health health) {
        destructableObjects.Remove(health);
    }
    #endregion

    #region Instance behaviour
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform explosionOrigin;

    private readonly float timeBetweenPathUpdatesWhenBlocked = 1f;

    private NavMeshAgent navMeshAgent;
    private bool isInDestroyObstaclesMode = false;
    private float timeuntilPathUpdateBlocked = 0f;

    public override void OnPhotonInstantiate(PhotonMessageInfo info) {
        base.OnPhotonInstantiate(info);
        Assert.IsNotNull(animator);
        Assert.IsNotNull(explosionOrigin);

        EnemyStats stats = EnemyStats.FromObjectArray(photonView.InstantiationData);

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = stats.movementSpeed;
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER) {
            navMeshAgent.enabled = false;
        }

        reachedThreshhold = navMeshAgent.radius * 1.1f;
    }

    protected override void Move(float distanceToTarget, float deltaTime) {
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER || currentTarget == null) {
            return;
        }

        // Try reset / open up path
        if(!navMeshAgent.hasPath || navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete) {
            if(timeuntilPathUpdateBlocked <= 0f) {
                timeuntilPathUpdateBlocked = timeBetweenPathUpdatesWhenBlocked;
                UpdateTarget();
            }
            else {
                timeuntilPathUpdateBlocked -= Time.deltaTime;
            }
            isInDestroyObstaclesMode = true;
            photonView.RPC(nameof(SetAnimationBoolRPC), RpcTarget.All, "IsDestructive", isInDestroyObstaclesMode);
        }

        // Exit open up mode
        if(navMeshAgent.hasPath && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) {
            isInDestroyObstaclesMode = false;
            photonView.RPC(nameof(SetAnimationBoolRPC), RpcTarget.All, "IsDestructive", isInDestroyObstaclesMode);
            debugText.text = $"Targeting {currentTarget.name} {distanceToTarget}m / {reachedThreshhold}m";
        }

        // Try damage obstacle to open up path
        if(isInDestroyObstaclesMode) {
            Health closestDestructableObstacle = GetClosestDestructableObstacle(transform.position);
            Assert.IsNotNull(closestDestructableObstacle);

            Vector3 obstaclePosition = GetClosestPoint(closestDestructableObstacle.transform);
            float distranceToObstacle = Vector3.Distance(obstaclePosition, transform.position);
            if(distranceToObstacle <= reachedThreshhold) {
                debugText.text = $"DESTRUCTIVE\nDying";
                closestDestructableObstacle.Damage(attackDamage);
                PhotonNetwork.Instantiate("Explosion", explosionOrigin.transform.position, Quaternion.identity);
                PhotonNetwork.Destroy(photonView);
                return;
            }
            else {
                debugText.text = $"DESTRUCTIVE\nTargeting {currentTarget.name} {distranceToObstacle}m / {reachedThreshhold}m " +
                    $"hasPath={navMeshAgent.hasPath} pathStatus={navMeshAgent.pathStatus}";
            }
        }
    }

    protected override void OnTargetLost() {
        navMeshAgent.isStopped = true;
    }

    protected override void OnTargetSet() {
        navMeshAgent.isStopped = false;
        navMeshAgent.destination = currentTarget.transform.position;
    }

    private Health GetClosestDestructableObstacle(Vector3 position) {
        return GetClosestHealth(destructableObjects, position);
    }

    [PunRPC]
    private void SetAnimationBoolRPC(string name, bool value) {
        animator.SetBool(name, value);
    }

    #endregion
}
