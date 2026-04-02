using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private string targetSpawnID; //Name of spawnpoint where the player should spawn

    public void SetSpawnID(string id)
    {
        targetSpawnID = id;
    }

    public void Awake()
    {
      if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
      else
        {
            Destroy(gameObject);
        }
    }

    //Finish doing the rest of this
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach (var point in spawnPoints)
        {
            if(point.spawnID == targetSpawnID)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Rigidbody rb = player.GetComponent<Rigidbody>();
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    player.transform.position = point.transform.position;
                    player.transform.rotation = point.transform.rotation;
                }
                break;
            }
        }
    }
}
