using Unity.Netcode;
using UnityEngine;

public class DisableOtherCameras : NetworkBehaviour
{
    public Camera playerCamera;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient && !IsOwner && playerCamera != null)
        {
            playerCamera.enabled = false;
        }
    }
}