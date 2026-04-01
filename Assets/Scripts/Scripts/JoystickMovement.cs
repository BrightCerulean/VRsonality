using UnityEngine;

public class JoystickMovement : MonoBehaviour
{
    public float speed = 10f;
    public Transform cameraTransform;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            Vector3 direction = cameraTransform.forward * v + cameraTransform.right * h;
            direction.y = 0f;
            direction.Normalize();
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}