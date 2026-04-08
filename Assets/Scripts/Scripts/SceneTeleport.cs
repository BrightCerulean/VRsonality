using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : Portal
{
    public string sceneToLoad; // Set in Inspector e.g. "PastRoom"
    public string targetSpawnID; // Name of the spawnpoint in the new scene where the player should spawn

    public override void Activate(GameObject player, PortalTrigger trigger)
    {
        SpawnManager.Instance.SetSpawnID(targetSpawnID);
        SceneManager.LoadScene(sceneToLoad);
    }
}
