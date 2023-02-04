using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviourPun, IPunInstantiateMagicCallback {

    #region Static destructable obstacle / potential target management
    private static readonly HashSet<Health> destructableObjects = new();
    private static readonly HashSet<Health> potentialStaticTargets = new();

    public static void RegisterDestructableObstacle(Health health) {
        destructableObjects.Add(health);
    }

    public static void DeregisterDestructableObstacle(Health health) {
        destructableObjects.Remove(health);
    }

    public static void RegisterStaticTarget(Health health) {
        potentialStaticTargets.Add(health);
    }

    public static void DeregisterTarget(Health health) {
        potentialStaticTargets.Remove(health);
    }

    public static bool HasPotentialTargets() {
        return potentialStaticTargets.Count > 0;
    }
    #endregion

    #region Instance behaviour
    private NavMeshAgent navMeshAgent;
    public Health Health { get; private set; }
    [SerializeField]
    private Transform hitPos;
    public Transform HitPos { get => hitPos; }

    private readonly float damageOutput = 25f;
    private Health currentTarget = null;
    private bool isInDestroyObstaclesMode = false;

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        navMeshAgent = GetComponent<NavMeshAgent>();
        Health = GetComponent<Health>();
        Health.Init(5f);
        Health.AddHealthListener(CheckHealth);
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER) {
            navMeshAgent.enabled = false;
        }
    }

    private void Update() {
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER) {
            return;
        }

        // Try damage target
        if(currentTarget != null && Vector3.Distance(currentTarget.transform.position, transform.position) <= navMeshAgent.radius) {
            currentTarget.Damage(damageOutput);
            PhotonNetwork.Destroy(photonView);
            return;
        }

        // Try reset / open up path
        if(!navMeshAgent.hasPath || navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete) {
            if(!isInDestroyObstaclesMode) {
                UpdateTarget();
            }
            isInDestroyObstaclesMode = true;
        }

        // Exit open up mode
        if(navMeshAgent.hasPath && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) {
            isInDestroyObstaclesMode = false;
        }

        // Try damage obstacle to open up path
        if(isInDestroyObstaclesMode) {
            Health closestDestructableObstacle = GetClosestDestructableObstacle(transform.position);
            Assert.IsNotNull(closestDestructableObstacle);

            Vector3 obstaclePosition = GetClosestPoint(closestDestructableObstacle.transform);
            float distranceToObstacle = Vector3.Distance(obstaclePosition, transform.position);
            if(distranceToObstacle <= navMeshAgent.radius * 1.1f) {
                closestDestructableObstacle.Damage(damageOutput);
                PhotonNetwork.Destroy(photonView);
                return;
            }
        }

        // Reset target on target death etc.
        if(!potentialStaticTargets.Contains(currentTarget)) {
            currentTarget = null;
            navMeshAgent.isStopped = true;
        }

        // Try set new target
        if(currentTarget == null) {
            UpdateTarget();
        }
    }

    private void UpdateTarget() {
        currentTarget = GetClosestTarget(transform.position);

        if(currentTarget != null) {
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = currentTarget.transform.position;
        }
    }

    private Health GetClosestTarget(Vector3 position) {
        return GetClosestHealth(potentialStaticTargets, position);
    }

    private Health GetClosestDestructableObstacle(Vector3 position) {
        return GetClosestHealth(destructableObjects, position);
    }

    private Health GetClosestHealth(HashSet<Health> healths, Vector3 position) {
        Health closestHealth = null;
        float closestDistance = float.MaxValue;
        foreach(Health health in healths) {
            Vector3 targetPosition = GetClosestPoint(health.transform);
            float targetDistance = Vector2.Distance(
                new Vector2(targetPosition.x, targetPosition.z),
                new Vector2(position.x, position.z)
            );
            if(targetDistance < closestDistance) {
                closestHealth = health;
                closestDistance = targetDistance;
            }
        }
        return closestHealth;
    }

    private Vector3 GetClosestPoint(Transform other) {
        Vector3 position = other.position;
        if(other.TryGetComponent(out Collider collider)) {
            position = collider.ClosestPointOnBounds(transform.position);
        }
        return position;
    }

    private void CheckHealth(float currentHealth, float maxHealth) {
        if(currentHealth <= 0) {
            PhotonNetwork.Destroy(photonView);
        }
    }

    #endregion
}
