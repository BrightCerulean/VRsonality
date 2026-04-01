using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public Portal portal; //Set in Inspector to either SceneTeleport or RoomTeleport
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            portal.Activate(other.gameObject);
        }
    }
}
