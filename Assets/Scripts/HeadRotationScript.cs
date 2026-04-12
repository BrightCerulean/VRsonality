using Unity.Netcode;
using UnityEngine;

public class HeadRotationSync : NetworkBehaviour
{
    [SerializeField] Transform headTransform;

    private NetworkVariable<Quaternion> _headRot = new(
        Quaternion.identity,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    void Update()
    {
        if (!IsSpawned) return;

        if (IsOwner)
            _headRot.Value = headTransform.rotation;
        else
            headTransform.rotation = _headRot.Value;
    }
}