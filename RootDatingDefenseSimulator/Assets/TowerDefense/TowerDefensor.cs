using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDefensor : MonoBehaviour {
    [SerializeField]
    private GameObject prefab;

    private bool init = false;
    private Camera cam;
    private int raycastLayerMask;

    private void Update() {
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER || !init) {
            return;
        }

        if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, raycastLayerMask)) {
            if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                GameObject go = PhotonNetwork.Instantiate(prefab.name, hit.point, Quaternion.identity);
                go.GetComponent<Tree>().Init(DatingHandler.GenerateRandomTreeStats());
            }
        }
    }

    public void Init() {
        init = true;
        cam = Camera.main;
        raycastLayerMask = 1 << 6;
    }
}
