using UnityEngine;

public class Teleporting : MonoBehaviour
{
    public Transform playerTransform;
    public float playerHeight = 1.8f;
    private string aButton = "js5";

    //public void TeleportTo(Vector3 point)
    public void TeleportTo(Vector3 point, bool isInteractable)
    {
        // To distinguish between floor and object and teleport only if it is floor
        if (isInteractable) return;
        if (!Input.GetButtonDown(aButton) && !Input.GetKeyDown(KeyCode.T)) 
        {
            return;
        }
        playerTransform.position = point + Vector3.up * playerHeight;
        Debug.Log("Teleported to : " + point);

    }

}