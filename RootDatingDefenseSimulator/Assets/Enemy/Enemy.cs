using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
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

    private static Health GetClosestTarget(Vector3 position) {
        Health closestTarget = null;
        float closestDistance = float.MaxValue;
        foreach(Health target in potentialStaticTargets) {
            float targetDistance = Vector3.Distance(target.transform.position, position);
            if(targetDistance < closestDistance) {
                closestTarget = target;
            }
        }
        return closestTarget;
    }
    #endregion

    #region Instance behaviour
    private NavMeshAgent navMeshAgent;

    private readonly float damageOutput = 25f;
    private Health currentTarget = null;

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        navMeshAgent = GetComponent<NavMeshAgent>();
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
        }

        // Reset target on taret death etc.
        if(!potentialStaticTargets.Contains(currentTarget)) {
            currentTarget = null;
            navMeshAgent.isStopped = true;
        }

        // Try set new target
        if(currentTarget == null) {
            currentTarget = GetClosestTarget(transform.position);

            if(currentTarget != null) {
                navMeshAgent.isStopped = false;
                navMeshAgent.destination = currentTarget.transform.position;
            }
        }
    }
    #endregion
}
