using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDefensor : MonoBehaviour {
    [SerializeField]
    private GameObject prefab;

    private bool init = false;
    private Camera cam;
    private int raycastLayerMask;
    private static TreeButton currentlySelected;

    private void Update() {
        if(GameLogic.PlayerRole != PlayerRole.TOWER_DEFENSER || !init) {
            return;
        }

        if(currentlySelected != null) {
            if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, raycastLayerMask)) {
                if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    GameObject go = PhotonNetwork.Instantiate(prefab.name, hit.point, Quaternion.identity);
                    go.GetComponent<TreeConscript>().Init(currentlySelected.PopTreeStats());
                    currentlySelected.Highlight(false);
                    currentlySelected = null;
                }
            }
        }
    }

    public void Init() {
        init = true;
        cam = Camera.main;
        raycastLayerMask = 1 << 6;
    }

    public static void SetTreeSelected(TreeButton treeButton) {
        if(currentlySelected != null) {
            currentlySelected.Highlight(false);
        }

        if(treeButton.IsEmpty) {
            return;
        }

        currentlySelected = treeButton;
        treeButton.Highlight(true);
    }
}
