using System.Net.NetworkInformation;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private string targetSpawnID; //Name of spawnpoint where the player should spawn

    public void SetSpawnID(string id)
    {
        targetSpawnID = id;
        Debug.Log("TARGET SPAWN ID SET TO: " + id);
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
            return;
        }
    }

    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("SCENE LOADING");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("SCENE UNLOADING");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("FINDING SPAWN POINTS");
        SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (var point in spawnPoints)
        {
            if(point.spawnID == targetSpawnID)
            {
                Debug.Log("TARGET SPAWN POINT LOCATED");
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Debug.Log("TELEPORTING PLAYER");
                    CharacterController cc = player.GetComponent<CharacterController>();
                    if (cc != null)
                    {
                        cc.enabled = false;
                    }
                    player.transform.position = point.transform.position;
                    player.transform.rotation = point.transform.rotation;
                    if (cc != null)
                    {
                        cc.enabled = true;
                    }
                    targetSpawnID = null;
                    Debug.Log("DONE");
                }
                break;
            }
        }
        StartCoroutine(FadeInAfterLoad());
    }
    private IEnumerator FadeInAfterLoad()
    {
        yield return null; 
        yield return ScreenFader.Instance.FadeFromBlack();
    }
}
