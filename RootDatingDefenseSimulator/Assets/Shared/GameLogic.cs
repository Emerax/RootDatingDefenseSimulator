using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameLogic : MonoBehaviourPunCallbacks {
    [SerializeField]
    private FunGoernForest forest;
    [SerializeField]
    private TowerDefensor towerDefensor;

    public static PlayerRole PlayerRole { get; private set; } = PlayerRole.NONE;
    public static GameState GameState { get; private set; }

    private bool mainPlayersAssigned = false;

    private void Start() {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("You: Hello!");
            photonView.RPC(nameof(SayHelloRPC), RpcTarget.Others);
        }
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom() {
        if(PhotonNetwork.IsMasterClient) {
            Debug.Log("Created room!");
            //You are host!
            AssignPlayerTypeRPC(PlayerRole.TOWER_DEFENSER);
        }

        Debug.Log($"Joined room as player {PhotonNetwork.LocalPlayer}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        if(PlayerRole is not PlayerRole.TOWER_DEFENSER) {
            return;
        }

        if(!mainPlayersAssigned) {
            photonView.RPC(nameof(AssignPlayerTypeRPC), newPlayer, PlayerRole.DATING_SIMULATOR);
            mainPlayersAssigned = true;
            return;
        }

        //All others become spectators. Or chat if they are mobile.
        photonView.RPC(nameof(AssignPlayerTypeRPC), newPlayer, PlayerRole.SPECTATOR);
    }

    private void OnPlayerTypeAssigned() {
        string newNickName;
        switch(PlayerRole) {
            case PlayerRole.TOWER_DEFENSER:
                newNickName = "Tower Defenser";
                break;
            case PlayerRole.DATING_SIMULATOR:
                mainPlayersAssigned = true;
                newNickName = "Dating Simulator";
                break;
            case PlayerRole.SPECTATOR:
                mainPlayersAssigned = true;
                newNickName = "Spectator";
                break;
            case PlayerRole.NONE:
            default:
                newNickName = "ERROR";
                Debug.LogError($"Trying to start game without a proper player type D: Type:{PlayerRole}");
                break;
        }

        PhotonNetwork.LocalPlayer.NickName = newNickName;
        InitGame();
    }

    private void InitGame() {
        forest.Init(GameOverRPC);
        towerDefensor.Init();
    }

    private void OnGameOver() {
        photonView.RPC(nameof(GameOverRPC), RpcTarget.All);
    }

    [PunRPC]
    private void AssignPlayerTypeRPC(PlayerRole type) {
        Debug.Log($"You recieved role: {type}");
        PlayerRole = type;
        OnPlayerTypeAssigned();
    }

    [PunRPC]
    private void GameOverRPC() {
        Debug.Log("GAME OVER MAAAN");
    }

    [PunRPC]
    private void SayHelloRPC(PhotonMessageInfo info) {
        Debug.Log($"{info.Sender.NickName}: Hello!");
    }
}
