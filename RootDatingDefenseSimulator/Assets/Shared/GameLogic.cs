using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GameLogic : MonoBehaviourPunCallbacks {
    [SerializeField]
    private ServerSettings photonServerSettings;
    [SerializeField]
    private FunGoernForest forest;
    [SerializeField]
    private TowerDefensor towerDefensor;
    [SerializeField]
    private WaveManager waveManager;
    [SerializeField]
    private DatingHandler UI;
    [SerializeField]
    private List<GameObject> gameStartElements;
    [SerializeField]
    private TMP_InputField roomNameField;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Canvas gameOverCanvas;
    [SerializeField]
    private TMP_Text gameVersionText;
    [SerializeField]
    private string hightlightColor;

    public static bool isGameOver = false;

    public static PlayerRole PlayerRole { get; private set; } = PlayerRole.NONE;
    public static GameState GameState { get; private set; }

    private string roomName = "Public";
    private bool mainPlayersAssigned = false;
    private bool isDatingSimPlayerReady = false;

    private void Awake() {
        Assert.IsNotNull(photonServerSettings);
        Assert.IsNotNull(forest);
        Assert.IsNotNull(towerDefensor);
        Assert.IsNotNull(waveManager);
        Assert.IsNotNull(UI);
        Assert.IsTrue(gameStartElements.Count > 0);
        Assert.IsNotNull(roomNameField);
        Assert.IsNotNull(startButton);
        Assert.IsNotNull(gameOverCanvas);
        Assert.IsNotNull(gameVersionText);
        PhotonNetwork.IsMessageQueueRunning = false;
        gameOverCanvas.gameObject.SetActive(false);
        foreach(GameObject element in gameStartElements) {
            element.SetActive(true);
        }
        UI.Clear();
        gameVersionText.SetText($"v {photonServerSettings.AppSettings.AppVersion}");

        isGameOver = false;
    }

    private void Start() {
        startButton.onClick.AddListener(StartGame);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Return)) {
            StartGame();
        }
    }

    private void StartGame() {
        if(!string.IsNullOrEmpty(roomNameField.text)) {
            roomName = roomNameField.text;
        }
        PhotonNetwork.ConnectUsingSettings();

        foreach(GameObject element in gameStartElements) {
            element.SetActive(false);
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log($"Connected with app version {PhotonNetwork.AppVersion}");
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom() {
        if(PhotonNetwork.IsMasterClient) {
            Debug.Log("Created room!");
            //You are host!
            AssignPlayerTypeRPC(PlayerRole.TOWER_DEFENSER);
        }

        Debug.Log($"Joined room {PhotonNetwork.CurrentRoom.Name} as player {PhotonNetwork.LocalPlayer}");
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
        Debug.Log($"Initializing game...");
        forest.Init(OnGameOver);
        towerDefensor.Init();
        UI.Initialize($"Room  <color={hightlightColor}>{roomName}\n{PhotonNetwork.LocalPlayer.NickName}</color>");

        if(PlayerRole == PlayerRole.TOWER_DEFENSER) {
            UI.SetStatusText($"Waiting for <color={hightlightColor}>Dating Simulator</color> player . . .");
        }
        else if(PlayerRole == PlayerRole.TOWER_DEFENSER) {
            UI.SetStatusText($"Waiting for <color={hightlightColor}>Tower Defenser</color> player . . .");
        }
        else {
            UI.SetStatusText("Waiting for game to start . . .");
        }

        PhotonNetwork.IsMessageQueueRunning = true;
        photonView.RPC(nameof(OnPlayerReady), RpcTarget.All, PlayerRole);
    }


    [PunRPC]
    private void OnPlayerReady(PlayerRole newPlayerRole) {
        Debug.Log($"Player {newPlayerRole} ready!");

        if(newPlayerRole == PlayerRole.DATING_SIMULATOR) {
            isDatingSimPlayerReady = true;
        }

        if(PlayerRole == PlayerRole.TOWER_DEFENSER) {
            if(isDatingSimPlayerReady) {
                photonView.RPC(nameof(GameStartRPC), RpcTarget.All);
            }
            else {
                Debug.Log($"Waiting for Dating Simulator player to join...");
            }
        }
    }

    [PunRPC]
    private void GameStartRPC() {
        UI.SetStatusText($"Waiting for initial wave . . .");
        waveManager.OnNewWave += OnNewWave;
        waveManager.Init();
        Debug.Log($"Game started!");
    }

    private void OnNewWave(int waveIndex) {
        photonView.RPC(nameof(OnNewWaveRPC), RpcTarget.All, waveIndex);
    }

    [PunRPC]
    private void OnNewWaveRPC(int waveIndex) {
        UI.SetStatusText($"Wave  <color={hightlightColor}>{waveIndex}</color>");
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
        isGameOver = true;
        gameOverCanvas.gameObject.SetActive(true);
    }

    [PunRPC]
    private void SayHelloRPC(PhotonMessageInfo info) {
        Debug.Log($"{info.Sender.NickName}: Hello!");
    }
}
