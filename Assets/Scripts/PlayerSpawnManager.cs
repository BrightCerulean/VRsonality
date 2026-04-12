using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = (request, response) =>
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.Position = spawnPoint.position;
            response.Rotation = spawnPoint.rotation;
        };
    }
}