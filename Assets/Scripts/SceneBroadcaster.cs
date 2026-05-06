using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBroadcaster : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!PhotonNetwork.IsConnected) return;

        Hashtable props = new Hashtable { { "scene", scene.name } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        Debug.Log($"[Scene] Broadcast scene: {scene.name}");
    }
}