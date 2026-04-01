using UnityEngine;

public class ColorObject : MonoBehaviour
{
    public GameObject targetObject;
    public bool isCube = false;
    public bool isSphere = false;
    void Start()
    {
        if (isCube == true)
        targetObject.GetComponent<Renderer>().material.color = Color.red;
        if (isSphere == true)
        targetObject.GetComponent<Renderer>().material.color = Color.yellow;
    }
    void Update()
{
    for (int i = 0; i < 20; i++)
    {
        if (Input.GetKeyDown((KeyCode)(360 + i)))
            Debug.Log("JoystickButton" + i + " pressed");
    }
}
}