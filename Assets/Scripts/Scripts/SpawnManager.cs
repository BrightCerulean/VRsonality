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

    //Finish doing the rest of this
}
