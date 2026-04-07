using Unity.Netcode;
using UnityEngine;

public class DisableOtherCameras : NetworkBehaviour
{
    public Camera playerCamera;

    void Start()
    {
        if (!IsOwner)
        {
            playerCamera.enabled = false;
        }
    }
}