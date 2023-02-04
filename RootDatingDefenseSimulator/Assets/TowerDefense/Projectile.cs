using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField]
    private float flightSpeed = 5f;
    [SerializeField]
    private float rotationSpeed = 120f;
    [SerializeField]
    private Transform visualsTransform;
    [SerializeField]
    private Enemy target;
    [SerializeField]
    private GameSettings settings;

    private float damage;
    private float spawnTime;

    private Vector3 rotationAngle;
    private Vector3 prevTravelDirection;

    public void Init(Enemy target, float damage) {
        this.target = target;
        this.damage = damage;
        rotationAngle = Random.onUnitSphere.normalized;
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update() {
        if(Time.time - spawnTime > settings.projectileLifetime) {
            PhotonNetwork.Destroy(gameObject);
        }

        visualsTransform.Rotate(rotationSpeed * Time.deltaTime * rotationAngle);
        if(target != null) {
            Vector3 towardTarget = (target.HitPos.position - transform.position).normalized;
            transform.Translate(flightSpeed * Time.deltaTime * towardTarget, Space.World);
            prevTravelDirection = towardTarget;
        }
        else {
            transform.Translate(flightSpeed * Time.deltaTime * prevTravelDirection);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Enemy>() is Enemy enemy) {
            enemy.Health.Damage(damage);
            PhotonNetwork.Destroy(gameObject);
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
