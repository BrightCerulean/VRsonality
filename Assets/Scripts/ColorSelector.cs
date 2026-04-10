using UnityEngine;

public class ColorSelector : MonoBehaviour
{
    public Color color;
    public string colorName;
    public GameObject startButton;

    private Vector3 originalPosition;
    private bool isSelected = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    public void OnSelect()
    {
        // Deselect all others
        foreach (var selector in FindObjectsByType<ColorSelector>(FindObjectsSortMode.None))
        {
            selector.isSelected = false;
            selector.transform.position = selector.originalPosition;
        }

        // Select this one
        isSelected = true;
        transform.position = originalPosition + Vector3.up * 0.3f;
        GameManager.Instance.SetPlayerColor(color, colorName);
        Debug.Log($"Color selected: {colorName}");

        // Activate start button
        if (startButton != null)
            startButton.SetActive(true);
    }
}