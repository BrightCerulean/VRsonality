using UnityEngine;

public class Highlight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Outline outline;
    public bool isHighlighted;
    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        isHighlighted = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Enable()
    {
        outline.enabled = true;
        isHighlighted = true;
    }

    public void Disable()
    {
        outline.enabled = false;
        isHighlighted = false;
    }
}
