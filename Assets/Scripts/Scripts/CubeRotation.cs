using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CubeRotation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float rotateSpeed = 90f;
    public Vector3 rotateAxis = Vector3.up;
    private bool isPointing = false;
    private string bButton = "js10";

    void Update()
    {
        if (isPointing && (Input.GetButtonDown(bButton)||Input.GetKey(KeyCode.R)))
        {
            transform.Rotate(rotateAxis * rotateSpeed * Time.deltaTime);
        }

    }

    public void OnPointerEnter(PointerEventData eventData) { isPointing = true; }
    public void OnPointerExit(PointerEventData eventData) { isPointing = false; }
}