using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 2f;
    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + openAngle,
            transform.eulerAngles.z
        );
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            isOpen ? openRot : closedRot,
            Time.deltaTime * speed
        );
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}