using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class Enemy : MonoBehaviourPun, IPunInstantiateMagicCallback {

    #region Static potential target management
    private static readonly HashSet<Health> potentialStaticTargets = new();

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
    public Health Health { get; private set; }
    [SerializeField]
    private GameSettings gameSettings;
    [SerializeField]
    private Transform hitPos;
    [SerializeField]
    protected TMP_Text debugText;
    public Transform HitPos { get => hitPos; }

    protected Health currentTarget = null;
    protected float attackDamage;
    protected float reachedThreshhold;

    public virtual void OnPhotonInstantiate(PhotonMessageInfo info) {
        Assert.IsNotNull(gameSettings);
        Assert.IsNotNull(hitPos);
        Assert.IsNotNull(debugText);

        EnemyStats stats = EnemyStats.FromObjectArray(photonView.InstantiationData);

        attackDamage = stats.attackDamage;
        Health = GetComponent<Health>();
        Health.Init(stats.health);
        Health.AddHealthListener(CheckHealth);

        Vector3 position = transform.position;
        position.y = stats.verticalOffset;
        transform.position = position;

        if(TryGetComponent(out CapsuleCollider collider)) {
            reachedThreshhold = collider.radius * 1.1f;
        }
    }

    protected virtual void Update() {
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER) {
            debugText.gameObject.SetActive(false);
            return;
        }

        // Try damage target
        float distanceToTarget = float.NaN;
        if(currentTarget != null) {
            Vector3 toTarget = currentTarget.transform.position - transform.position;
            toTarget.y = 0f;
            distanceToTarget = toTarget.magnitude;
            if(distanceToTarget <= reachedThreshhold) {
                debugText.text = $"Dying";
                currentTarget.Damage(attackDamage);
                PhotonNetwork.Destroy(photonView);
                return;
            }
        }

        Move(distanceToTarget, Time.deltaTime);

        // Reset target on target death etc.
        if(!potentialStaticTargets.Contains(currentTarget)) {
            currentTarget = null;
            OnTargetLost();
            debugText.text = $"No target";
        }

        // Try set new target
        if(currentTarget == null) {
            UpdateTarget();
            debugText.text = $"Fresh target";
        }
    }
    protected virtual void Move(float distanceToTarget, float deltaTime) {

    }

    protected virtual void OnTargetLost() {

    }

    protected virtual void OnTargetSet() {

    }

    protected virtual void UpdateTarget() {
        currentTarget = GetClosestTarget(transform.position);
        if(currentTarget != null) {
            OnTargetSet();
        }
    }

    private Health GetClosestTarget(Vector3 position) {
        return GetClosestHealth(potentialStaticTargets, position);
    }

    protected Health GetClosestHealth(HashSet<Health> healths, Vector3 position) {
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

    protected Vector3 GetClosestPoint(Transform other) {
        Vector3 position = other.position;
        if(other.TryGetComponent(out Collider collider)) {
            position = collider.ClosestPointOnBounds(transform.position);
        }
        return position;
    }

    private void CheckHealth(float currentHealth, float maxHealth) {
        if(GameLogic.PlayerRole is not PlayerRole.TOWER_DEFENSER) {
            return;
        }

        if(currentHealth <= 0) {
            PhotonNetwork.Destroy(photonView);
        }
    }

    #endregion
}
