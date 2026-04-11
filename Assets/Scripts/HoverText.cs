using UnityEngine;
using TMPro;

public class HoverText : MonoBehaviour
{
    public GameObject textObject;
    public TextMeshProUGUI textComponent;

    void Start()
    {
        SetVisible(false);
    }
    public void SetVisible(bool state)
    {
        if (textObject != null)
            textObject.SetActive(state);
        if (state && textComponent != null)
        {
            textComponent.color = Color.black;
        }
    }

    public void SetOutline(bool enabled)
    {
        if (textComponent == null) return;
        
        if (enabled && GameManager.Instance != null)
        {
            textComponent.outlineWidth = 0.2f;
            textComponent.outlineColor = GameManager.Instance.playerColor;
        }
        else
        {
            textComponent.outlineWidth = 0f;
        }
    }
}
