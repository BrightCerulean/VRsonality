using UnityEngine;
using System.Collections;
/*
public class RespawnManager : MonoBehaviour
{
    // The original Prefab to spawn (assign in the Inspector)
    public GameObject objectPrefab;
    
    // Position where the object should spawn
    public Transform spawnPoint;

    // Time to wait before respawning
    public float respawnDelay = 2.0f;

    // Reference to the current instance in the scene
    private GameObject currentObject;

    void Start()
    {
        Spawn();
    }

    public void Respawn()
    {
        // 1. Destroy the current object
        if (currentObject != null)
        {
            Destroy(currentObject);
        }

        // 2. Start timer to spawn a new one
        StartCoroutine(RespawnTimer());
    }

    private IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(respawnDelay);
        Spawn();
    }

    private void Spawn()
    {
        currentObject = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
*/


public class ScriptExample : MonoBehaviour
{
    void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    void DestroyScriptInstance()
    {
        // Removes this script instance from the game object
        Destroy(this);
    }

    void DestroyComponent()
    {
        // Removes the rigidbody from the game object
        Destroy(GetComponent<Rigidbody>());
    }

    void DestroyObjectDelayed()
    {
        // Kills the game object in 5 seconds after loading the object
        Destroy(gameObject, 5);
    }

    // When the user presses Ctrl, it will remove the
    // BoxCollider component from the game object
    void Update()
    {
        if (Input.GetButton("Fire1") && GetComponent<BoxCollider>())
        {
            Destroy(GetComponent<BoxCollider>());
        }
    }
}