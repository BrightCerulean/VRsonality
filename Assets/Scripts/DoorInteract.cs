using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public DoorOpen door;

    // Call this from your reticle/button press
    public void OnSelect()
    {
        door.ToggleDoor();
    }
}