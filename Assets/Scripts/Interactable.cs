using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public Outline outline;
    public HoverText hoverText;

    private bool isHovered = false;
    private bool isSelected = false;
    private bool isLocked = false;
    void Awake()
    {
        SetHover(false);
    }

    public void SetHover(bool state)
    {
        if (isSelected || isLocked) return;

        isHovered = state;

        if (outline != null)
            outline.enabled = state;

        if (hoverText != null)
            hoverText.SetVisible(state);
    }

    public void Select()
    {
        if (isLocked) return;

        isSelected = true;
        isHovered = false;

        if (outline != null)
            outline.enabled = true;

        if (hoverText != null)
            hoverText.SetVisible(true);
    }
    public void Lock()
    {
        isLocked = true;
        isHovered = false;

        if (outline != null)
            outline.enabled = false;

        if (hoverText != null)
            hoverText.SetVisible(false);
    }
    public void ResetInteractable()
    {
        isHovered = false;
        isSelected = false;
        isLocked = false;

        if (outline != null)
            outline.enabled = false;

        if (hoverText != null)
            hoverText.SetVisible(false);
    }
}
