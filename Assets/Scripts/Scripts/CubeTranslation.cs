using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
public class CubeTranslation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float moveSpeed = 2f;
    public Vector3 moveDirection = Vector3.right;
    private bool isPointing = false;
    private string bButton = "js10";


    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

   void Update()
    {
        if (isPointing && (Input.GetButtonDown(bButton)||Input.GetKey(KeyCode.M)))
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
        Debug.Log("isPointing: " + isPointing);
        Debug.Log("Button: " + Input.GetButton(bButton));
    }
     /*void Update()
    {
        if (isPointing && Input.GetButton(bButton))
        {
            rb.linearVelocity = Vector3.right * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }*/

    public void OnPointerEnter(PointerEventData eventData) { isPointing = true; }
    public void OnPointerExit(PointerEventData eventData) { isPointing = false; }
}
