using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerNetworkSetup : NetworkBehaviour
{
    [SerializeField] XRCardboardController cardboardController;
    [SerializeField] CharacterMovement characterMovement;
    [SerializeField] Assignment2Interactions interactions;
    [SerializeField] Camera playerCamera;
    [SerializeField] AudioListener audioListener;
    [SerializeField] GameObject avatarMesh;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            avatarMesh.SetActive(false);
            return;
        }

        cardboardController.enabled = false;
        characterMovement.enabled = false;
        interactions.enabled = false;
        playerCamera.enabled = false;
        audioListener.enabled = false;

        EventSystem remoteES = GetComponentInChildren<EventSystem>();
        if (remoteES != null)
            Destroy(remoteES.gameObject);
    }
}