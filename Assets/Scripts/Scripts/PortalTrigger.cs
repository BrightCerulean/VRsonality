using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public Portal portal; //Set in Inspector to either SceneTeleport or RoomTeleport
    bool hasTeleported = false; //Teleport cooldown
    void OnTriggerStay(Collider other)
    {
        //Debug.Log("PORTAL HIT BY: " + other.name);
        //Debug.Log("OBJECT TAG IS: " + other.tag);
        //Debug.Log("hasTeleported is currently false");
        //Debug.Log(hasTeleported);
        //Debug.Log(!hasTeleported);
        if (other.CompareTag("Player") && !hasTeleported)
        {
            hasTeleported = true;
            //Debug.Log("hasTeleported is now true");
            //Debug.Log("Player detected. Using: " + portal);
            portal.Activate(other.gameObject, this);
            //hasTeleported = false;
            //Debug.Log("hasTeleported is now false");
        }
    }
    public void ResetTeleport()
    {
        hasTeleported = false;
    }
}
