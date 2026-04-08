using UnityEngine;

public class RoomTeleport : Portal
{
    public Transform targetSpawnPoint; // Set in Inspector to where Player should spawn after entering portal
    public override void Activate(GameObject player, PortalTrigger trigger)
    {
        //Debug.Log("RUNNING ROOM TELEPORT SCRIPT");
        //Debug.Log("Teleporting to: " + targetSpawnPoint.position);
        //Debug.Log("Player before: " + player.transform.position);
        //float dist = Vector3.Distance(player.transform.position, targetSpawnPoint.position);
        //Debug.Log("Distance to spawn: " + dist);
        player.transform.position = targetSpawnPoint.position;
        player.transform.rotation = targetSpawnPoint.rotation;
        trigger.ResetTeleport();
        //Debug.Log("Player after: " + player.transform.position);
    }
}
