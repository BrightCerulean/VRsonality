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
    public string hostIP = "192.168.1.207";
    public ushort port = 7777;

    private int selectedIndex = 0;
    private GameObject[] buttons;
    private float menuNavCooldown = 0.2f;
    private float lastMenuNavTime = 0f;
    private float connectAttemptTime = -1f;

    void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found");
            return;
        }
        buttons = new GameObject[] { hostButton, clientButton };
        HighlightButton(selectedIndex);

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            Debug.LogError($"Disconnected. ClientId: {clientId}");
        };

        NetworkManager.Singleton.OnTransportFailure += () =>
        {
            Debug.LogError("Transport failure!");
        };
    }

    void Update()
    {
        Debug.Log("Update running, B: " + Input.GetKeyDown(KeyCode.B));
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
        {
            gameObject.SetActive(false);
            return;
        }
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log("Connected as Client.");
            gameObject.SetActive(false);
            return;
        }

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
        Debug.Log($"StartHost result: {started}");
        Debug.Log($"IsHost: {NetworkManager.Singleton.IsHost} | IsServer: {NetworkManager.Singleton.IsServer} | IsListening: {NetworkManager.Singleton.IsListening}");

        Application.logMessageReceived += (log, stack, type) =>
        {
            System.IO.File.AppendAllText(
                Application.persistentDataPath + "/netlog.txt",
                $"[{type}] {log}\n"
            );
        }; 
        
        gameObject.SetActive(false);

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