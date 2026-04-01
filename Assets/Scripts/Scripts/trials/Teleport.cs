using UnityEngine;
using UnityEngine.EventSystems;

public class Teleport : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform playerTransform;
    public float playerHeight = 1.6f;
    private bool isPointing = false;
    private Vector3 targetPosition;

    void Update()
    {
        // Continuously update target position while pointing at floor
        if (isPointing)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;
            }
        }

        if (isPointing && (Input.GetButtonDown("js11")||Input.GetKey(KeyCode.T)))
        {
            playerTransform.position = targetPosition + Vector3.up * playerHeight;
            Debug.Log("Teleported to: " + targetPosition);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) { isPointing = true; }
    public void OnPointerExit(PointerEventData eventData) { isPointing = false; }
}