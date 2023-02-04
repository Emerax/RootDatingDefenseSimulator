using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField]
    private float flightSpeed = 5f;
    [SerializeField]
    private float rotationSpeed = 120f;

    private Enemy target;
    private float damage;

    private Vector3 rotationAngle;
    private Vector3 prevTravelDirection;

    public void Init(Enemy target, float damage) {
        this.target = target;
        this.damage = damage;
        rotationAngle = Random.onUnitSphere.normalized;
    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(rotationSpeed * Time.deltaTime * rotationAngle);
        if(target != null) {
            Vector3 towardTarget = (transform.position - target.transform.position).normalized;
            transform.Translate(flightSpeed * Time.deltaTime * towardTarget);
            prevTravelDirection = towardTarget;
        }
        else {
            transform.Translate(prevTravelDirection * flightSpeed * Time.deltaTime);
        }
    }


}
