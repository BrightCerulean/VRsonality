using Unity.Netcode;
using UnityEngine;

public class HeadSync : NetworkBehaviour
{
    public Transform head;
    void Update()
    {
        if (!IsOwner) return;
        SubmitHeadTransformServerRpc(head.position, head.rotation);
    }

    [ServerRpc]
    void SubmitHeadTransformServerRpc(Vector3 pos, Quaternion rot)
    {
        UpdateHeadClientRpc(pos, rot);
    }

    [ClientRpc]
    void UpdateHeadClientRpc(Vector3 pos, Quaternion rot)
    {
        if (IsOwner) return;
        head.position = pos;
        head.rotation = rot;
    }
}