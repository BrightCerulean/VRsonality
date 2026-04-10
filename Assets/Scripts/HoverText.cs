using UnityEngine;

public class HoverText : MonoBehaviour
{
    public GameObject textObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetVisible(bool state)
    {
        if (textObject != null)
            textObject.SetActive(state);
    }

    /*
    void Start()
    {
        if (textObject != null)
            textObject.SetActive(false);
    }
    public void SetHover(bool state)
    {
        if (textObject != null)
            textObject.SetActive(state);
    }
    */
}
