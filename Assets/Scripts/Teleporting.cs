using UnityEngine;

public class Teleporting : MonoBehaviour
{
    public Transform playerTransform;
    public float playerHeight = 3f;

    void Update()
    {   
        Debug.Log("[Teleporting] Ray origin: " + Camera.main.transform.position + " direction: " + Camera.main.transform.forward);
        Debug.Log("[Teleporting] Checking for teleport input...");
        Debug.Log("[Teleporting] T pressed: " + Input.GetKeyDown(KeyCode.T));
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, 50f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    playerTransform.position = hit.point + Vector3.up * playerHeight;
                    Debug.Log("[Teleporting] Teleported to: " + hit.point);
                }
            }
        }
    }
}