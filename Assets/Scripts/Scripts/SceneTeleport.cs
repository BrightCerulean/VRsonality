using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : Portal
{
    public string sceneToLoad; // Set in Inspector e.g. "PastRoom"
    public Transform targetSpawnPoint; // Set in Inspector to where Player should spawn after entering portal

    public override void Activate(GameObject player)
    {
        SceneManager.LoadScene(sceneToLoad);
        //Set player position and rotation after scene transition
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.position = targetSpawnPoint.position;
        player.transform.rotation = targetSpawnPoint.rotation;
    }
}
