using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObstacleIgnoring : Enemy {

    private float movementSpeed;
    private float verticalOffset;

    public override void OnPhotonInstantiate(PhotonMessageInfo info) {
        base.OnPhotonInstantiate(info);

        EnemyStats stats = EnemyStats.FromObjectArray(photonView.InstantiationData);

        movementSpeed = stats.movementSpeed;
        verticalOffset = stats.verticalOffset;
    }

    protected override void Move(float distanceToTarget, float deltaTime) {
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER || currentTarget == null) {
            return;
        }

        Vector3 toTarget = currentTarget.transform.position - transform.position;
        toTarget.y = 0f;

        if(toTarget.sqrMagnitude < 0.05f) {
            debugText.text = $"TOO CLOSE\nTargeting {currentTarget.name} diff={toTarget} {distanceToTarget}m / {reachedThreshhold}m";
            return;
        }

        Vector3 movement = movementSpeed * deltaTime * toTarget.normalized;
        if(movement.sqrMagnitude > toTarget.sqrMagnitude) {
            movement = toTarget;
        }

        transform.position += movement;

        Vector3 position = transform.position;
        position.y = verticalOffset;
        transform.position = position;

        debugText.text = $"Targeting {currentTarget.name} diff={toTarget} {distanceToTarget}m / {reachedThreshhold}m";

        transform.LookAt(transform.position + toTarget, Vector3.up);
    }
}
