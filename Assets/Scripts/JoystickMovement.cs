using UnityEngine;

public class JoystickMovement : MonoBehaviour
{
    public float speed = 20f;
    public Transform cameraTransform;
    public bool invertHorizontal = false;
    public bool invertVertical = false;

    void Update()
    {
        float h = Input.GetAxis("Horizontal") * (invertHorizontal ? -1f : 1f);
        float v = Input.GetAxis("Vertical")   * (invertVertical   ? -1f : 1f);

        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            Vector3 direction = cameraTransform.forward * v + cameraTransform.right * h;
            direction.y = 0f;
            direction.Normalize();
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}