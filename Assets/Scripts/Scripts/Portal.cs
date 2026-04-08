using UnityEngine;

public abstract class Portal : MonoBehaviour
{
    public abstract void Activate(GameObject player, PortalTrigger trigger);
}