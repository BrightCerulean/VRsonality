using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Teleportation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform playerTransform;
    public float playerHeight = 1.6f;
    private string aButton = "js5";
    private bool isPointing = false;

    void Update()
    {
    if (isPointing && Input.GetButtonDown(aButton))
    {
        // Temporary editor test - shoots ray downward
        Ray ray = new Ray(Camera.main.transform.position, Vector3.down);
        //Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.Log("isPointing: " + isPointing);

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name + " Tag: " + hit.collider.tag);
            if (hit.collider.CompareTag("Floor"))
            {
                playerTransform.position = hit.point + Vector3.up * playerHeight;
                Debug.Log("Teleported!");
            }
        }
        else
        {
            Debug.Log("Raycast hit nothing!");
        }
    }
}

    public void OnPointerEnter(PointerEventData eventData) { isPointing = true; }
    public void OnPointerExit(PointerEventData eventData) { isPointing = false; }
}