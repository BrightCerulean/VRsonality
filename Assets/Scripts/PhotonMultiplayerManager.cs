using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonMultiplayerManager : MonoBehaviourPunCallbacks
{
    [Header("UI (optional for testing — can leave empty)")]
    public Text statusText;

    private const string ROOM_NAME = "VRQuizRoom";
    private const int MAX_PLAYERS = 4;

    private bool _avatarSpawned = false;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
        Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnConnectedToMaster()
    {
        Log("Connected. Joining room...");
        RoomOptions options = new RoomOptions { MaxPlayers = MAX_PLAYERS };
        PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Log($"Joined room. Players: {PhotonNetwork.CurrentRoom.PlayerCount}");
        SpawnAvatar();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Log($"{newPlayer.NickName} joined.");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Log($"{otherPlayer.NickName} left.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Log($"Disconnected: {cause}");
        _avatarSpawned = false;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        foreach (var avatar in FindObjectsByType<PlayerAvatar>(FindObjectsSortMode.None))
        {
            if (avatar.photonView.Owner == targetPlayer)
            {
                avatar.OnOwnerPropertiesChanged(changedProps);
                break;
            }
        }
    }


    void SpawnAvatar()
    {
        if (_avatarSpawned) return;
        _avatarSpawned = true;

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        Vector3 spawnPos = Vector3.zero;

        if (spawnPoints.Length > 0)
        {
            int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
            spawnPos = spawnPoints[index].transform.position;
        }

        PhotonNetwork.Instantiate("PlayerAvatar", spawnPos, Quaternion.identity);
    }


    public void LoadSceneForAll(string sceneName)
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(sceneName);
    }
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!PhotonNetwork.InRoom) return;

        _avatarSpawned = false;
        SpawnAvatar();

        Log($"Respawned avatar in {scene.name}");
    }


    void Log(string msg)
    {
        Debug.Log($"[Photon] {msg}");
        if (statusText != null) statusText.text = msg;
    }
}