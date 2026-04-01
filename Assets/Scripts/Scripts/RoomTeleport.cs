using UnityEngine;

public class RoomTeleport : Portal
{
    public Transform targetSpawnPoint; // Set in Inspector to where Player should spawn after entering portal
    public override void Activate(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = targetSpawnPoint.position;
        player.transform.rotation = targetSpawnPoint.rotation;
    }
}
