using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }
    // called by raycasting script when object is hit
    void OnEnable()
    {
        if (outline != null)
            outline.enabled = true;
    }
    // called by raycasting script when object is no longer hit
    void OnDisable()
    {
        if (outline != null)
            outline.enabled = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
    }
}



