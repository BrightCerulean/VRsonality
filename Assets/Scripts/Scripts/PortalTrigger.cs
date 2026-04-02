using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public Portal portal; //Set in Inspector to either SceneTeleport or RoomTeleport
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player inside trigger");
            portal.Activate(other.gameObject);
        }
    }
}
