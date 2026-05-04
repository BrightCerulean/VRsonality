using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class LANLobby : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject hostButton;
    public GameObject clientButton;

    [Header("Network")]
    public string hostIP = "127.0.0.1";
    public ushort port = 7777;

    [Header("Lobby")]
    public Canvas lobbyCanvas;
    public Camera lobbyCamera;

    /*
    [Header("Vivox")]
    public GameObject vivoxManager;
    */

    private int selectedIndex = 0;
    private GameObject[] buttons;
    private float menuNavCooldown = 0.2f;
    private float lastMenuNavTime = 0f;
    private float connectAttemptTime = -1f;

    void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found in scene!");
            return;
        }

        lobbyCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        lobbyCanvas.worldCamera = lobbyCamera;
        lobbyCanvas.planeDistance = 1f;

        buttons = new GameObject[] { hostButton, clientButton };
        HighlightButton(selectedIndex);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            Debug.LogError($"Disconnected. ClientId: {clientId}");
        };

        NetworkManager.Singleton.OnTransportFailure += () =>
        {
            Debug.LogError("Transport failure!");
        };
    }

    void OnClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        /*
        if (vivoxManager != null)
            vivoxManager.SetActive(true);
        */
        lobbyCamera.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    void Update()
    {
        if (connectAttemptTime > 0 && Time.time - connectAttemptTime > 5f)
        {
            Debug.LogError($"5s timeout. IsClient:{NetworkManager.Singleton.IsClient} | IsListening:{NetworkManager.Singleton.IsListening} | IsConnected:{NetworkManager.Singleton.IsConnectedClient}");
            connectAttemptTime = -1f;
        }

        bool goUp = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        bool goDown = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        float verticalInput = Input.GetAxis("Vertical");

        if (Time.time - lastMenuNavTime > menuNavCooldown)
        {
            if (verticalInput > 0.5f || goUp)
            {
                selectedIndex = Mathf.Max(0, selectedIndex - 1);
                HighlightButton(selectedIndex);
                lastMenuNavTime = Time.time;
            }
            else if (verticalInput < -0.5f || goDown)
            {
                selectedIndex = Mathf.Min(buttons.Length - 1, selectedIndex + 1);
                HighlightButton(selectedIndex);
                lastMenuNavTime = Time.time;
            }
        }

        if (B_Pressed())
        {
            Debug.Log($"Selected index: {selectedIndex} — calling {(selectedIndex == 0 ? "StartHost" : "StartClient")}");
            SelectCurrent();
        }
    }

    void HighlightButton(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Image img = buttons[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == index) ? Color.yellow : Color.white;
        }
    }

    void SelectCurrent()
    {
        if (selectedIndex == 0) StartHost();
        else StartClient();
    }

    void StartHost()
    {
        bool started = NetworkManager.Singleton.StartHost();
        Debug.Log($"StartHost result: {started} | IsHost: {NetworkManager.Singleton.IsHost}");
    }

    void StartClient()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(hostIP, port);
        NetworkManager.Singleton.StartClient();
        connectAttemptTime = Time.time;
    }

    bool B_Pressed()
    {
#if UNITY_EDITOR
        return Input.GetKeyDown(KeyCode.B);
#else
        return Input.GetKeyDown(KeyCode.JoystickButton5)
            || Input.GetKeyDown(KeyCode.B);
#endif
    }
}