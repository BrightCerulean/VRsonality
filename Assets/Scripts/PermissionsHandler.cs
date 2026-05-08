using UnityEngine;
using UnityEngine.Android;

public class PermissionsHandler : MonoBehaviour
{
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);
    }
}