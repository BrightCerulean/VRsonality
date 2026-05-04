using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    public Color outlineColor = new Color(1f, 0.85f, 0.2f, 1f);
    public float outlineWidth = 5f;
    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();

        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = outlineWidth;
        outline.enabled = false;
    }

    public void OnHoverEnter()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void OnHoverExit()
    {
        if (outline != null)
            outline.enabled = false;
    }
}