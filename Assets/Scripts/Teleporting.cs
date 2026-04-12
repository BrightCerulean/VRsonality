using UnityEngine;

public class Teleporting : MonoBehaviour
{
    public Transform playerTransform;
    public float playerHeight = 3f;
    public static string YButton;

    private void Awake()
    {
        if (Application.isEditor)//PC
        {
            YButton = "js0";
        }
        else//Android
        {
            YButton = "js2";
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetButtonDown(YButton))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, 50f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    playerTransform.position = hit.point + Vector3.up * playerHeight;
                    Debug.Log("[Teleporting] Teleported to: " + hit.point);
                    break;
                }
            }
        }
    }
}