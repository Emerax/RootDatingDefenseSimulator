using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    private void Update() {
        transform.rotation = Camera.main.transform.rotation;
    }
}
