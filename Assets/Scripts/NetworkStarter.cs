using Unity.Netcode;
using UnityEngine;

public class NetworkStarter : MonoBehaviour
{
    void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Debug.Log("Client connected: " + id);
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Started Host");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Started Client");
        }
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Started Host");
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Started Client");
    }
}
