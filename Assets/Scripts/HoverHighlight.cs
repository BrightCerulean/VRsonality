using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        Debug.Log("[HoverHighlight] Initialized on: " + gameObject.name);
    }

    public void OnHoverEnter()
    {
        Debug.Log("[HoverHighlight] OnHoverEnter: " + gameObject.name);
        transform.localScale = originalScale * 1.5f;
    }

    public void OnHoverExit()
    {
        Debug.Log("[HoverHighlight] OnHoverExit: " + gameObject.name);
        transform.localScale = originalScale;
    }
}