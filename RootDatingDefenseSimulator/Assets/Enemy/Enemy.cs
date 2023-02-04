using Photon.Pun;
using System.Collections.Generic;
using TMPro;
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
    private GameSettings gameSettings;
    [SerializeField]
    private Transform hitPos;
    [SerializeField]
    private TMP_Text debugText;
    public Transform HitPos { get => hitPos; }

    private readonly float timeBetweenPathUpdatesWhenBlocked = 1f;
    private Health currentTarget = null;
    private bool isInDestroyObstaclesMode = false;
    private float timeuntilPathUpdateBlocked = 0f;

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        Assert.IsNotNull(gameSettings);
        Assert.IsNotNull(hitPos);
        Assert.IsNotNull(debugText);
        navMeshAgent = GetComponent<NavMeshAgent>();
        Health = GetComponent<Health>();
        Health.Init(gameSettings.enemyHealth);
        Health.AddHealthListener(CheckHealth);
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER) {
            navMeshAgent.enabled = false;
        }
    }

    private void Update() {
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER) {
            debugText.gameObject.SetActive(false);
            return;
        }
        float reachedThreshhold = navMeshAgent.radius * 1.1f;

        // Try damage target
        float distanceToTarget = float.NaN;
        if(currentTarget != null) {
            distanceToTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
            if(distanceToTarget <= reachedThreshhold) {
                debugText.text = $"Dying";
                currentTarget.Damage(gameSettings.enemyDamage);
                PhotonNetwork.Destroy(photonView);
                return;
            }
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
        }

        // Exit open up mode
        if(navMeshAgent.hasPath && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) {
            isInDestroyObstaclesMode = false;
            debugText.text = $"Targeting {currentTarget} {distanceToTarget}m / {reachedThreshhold}m";
        }

        // Try damage obstacle to open up path
        if(isInDestroyObstaclesMode) {
            Health closestDestructableObstacle = GetClosestDestructableObstacle(transform.position);
            Assert.IsNotNull(closestDestructableObstacle);

            Vector3 obstaclePosition = GetClosestPoint(closestDestructableObstacle.transform);
            float distranceToObstacle = Vector3.Distance(obstaclePosition, transform.position);
            if(distranceToObstacle <= reachedThreshhold) {
                debugText.text = $"DESTRUCTIVE\nDying";
                closestDestructableObstacle.Damage(gameSettings.enemyDamage);
                PhotonNetwork.Destroy(photonView);
                return;
            }
            else {
                debugText.text = $"DESTRUCTIVE\nTargeting {currentTarget} {distranceToObstacle}m / {reachedThreshhold}m " +
                    $"hasPath={navMeshAgent.hasPath} pathStatus={navMeshAgent.pathStatus}";
            }
        }

        // Reset target on target death etc.
        if(!potentialStaticTargets.Contains(currentTarget)) {
            currentTarget = null;
            navMeshAgent.isStopped = true;
            debugText.text = $"No target";
        }

        // Try set new target
        if(currentTarget == null) {
            UpdateTarget();
            debugText.text = $"Fresh target";
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
