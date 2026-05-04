using UnityEngine;

public class CameraHeightSmoother : MonoBehaviour
{
    [Tooltip("Target world Y position for the player on scene load")]
    public float targetHeight = 1.6f;
    [Tooltip("How fast to lerp to the target height (higher = faster)")]
    public float smoothSpeed = 5f;

    private bool settling = true;

    void Update()
    {
        if (!settling) return;

        Vector3 pos = transform.position;
        float newY = Mathf.Lerp(pos.y, targetHeight, smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(pos.x, newY, pos.z);

        if (Mathf.Abs(newY - targetHeight) < 0.01f)
        {
            transform.position = new Vector3(pos.x, targetHeight, pos.z);
            settling = false;
        }
    }
}
