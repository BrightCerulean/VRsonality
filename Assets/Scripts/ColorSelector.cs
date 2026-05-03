using UnityEngine;

public class ColorSelector : MonoBehaviour
{
    public Color color;
    public string colorName;
    public GameObject startCanvas;
    public GameObject pickCanvas;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;

        if (startCanvas == null)
            startCanvas = GameObject.Find("CanvasStart");

        if (pickCanvas == null)
            pickCanvas = GameObject.Find("CanvasPick");

        if (startCanvas != null)
            startCanvas.SetActive(false);
    }

    public void OnSelect()
    {
        // Deselect all others
        foreach (var selector in FindObjectsByType<ColorSelector>(FindObjectsSortMode.None))
        {
            selector.transform.position = selector.originalPosition;
        }

        // Disable all other orbs
        foreach (var selector in FindObjectsByType<ColorSelector>(FindObjectsSortMode.None))
        {
            if (selector != this)
                selector.gameObject.SetActive(false);
        }

        // Select this one
        transform.position = originalPosition + Vector3.up * 0.3f;
        GameManager.Instance.SetPlayerColor(color, colorName);
        Debug.Log($"Color selected: {colorName}");

        // Hide pick canvas, show start canvas
        Debug.Log($"[ColorSelector] pickCanvas={(pickCanvas != null ? pickCanvas.name : "NULL")} startCanvas={(startCanvas != null ? startCanvas.name : "NULL")}");
        if (pickCanvas != null)
            pickCanvas.SetActive(false);

        if (startCanvas != null)
            startCanvas.SetActive(true);
    }
}