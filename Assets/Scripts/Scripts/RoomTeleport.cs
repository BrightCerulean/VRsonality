using UnityEngine;

public class RoomTeleport : Portal
{
    public Transform targetSpawnPoint; // Set in Inspector to where Player should spawn after entering portal
    protected override void PerformTeleport(GameObject player)
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        //Debug.Log("PLAYER'S CURRENT POSITION: " + player.transform.position);
        //Debug.Log("TARGET POSITION: " + targetSpawnPoint.position);
        player.transform.position = targetSpawnPoint.position;
        player.transform.rotation = targetSpawnPoint.rotation;
        //Debug.Log("PLAYER'S NEW POSITION: " + player.transform.position);
        if (cc != null) cc.enabled = true;
    }
}
