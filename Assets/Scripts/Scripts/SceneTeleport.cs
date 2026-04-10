using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTeleport : Portal
{
    public string sceneToLoad; // Set in Inspector e.g. "PastRoom"
    public string targetSpawnID; // Name of the spawnpoint in the new scene where the player should spawn

    protected override void PerformTeleport(GameObject player)
    {
        Debug.Log("SCENE TELEPORT TRIGGERED: " + gameObject.name);
        if (SpawnManager.Instance == null)
        {
            Debug.LogError("SpawnManager missing!");
            return;
        }
        SpawnManager.Instance.SetSpawnID(targetSpawnID);
        SceneManager.LoadScene(sceneToLoad);
    }
}
