using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    public Outline outline;
    public HoverText hoverText;
    public string sceneName;
    public string choiceLetter;

    private bool isHovered = false;
    private bool isSelected = false;
    private bool isLocked = false;
    public GameObject portal;
    public AudioClip selectSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        if (GameManager.Instance == null) return;
        string selected = GameManager.Instance.GetSelectionForScene(sceneName);
        if (selected != null)
        {
            if (selected == choiceLetter)
            {
                Select(); 
            }
            else
            {
                Lock(); 
            }
        }
    }
    void Awake()
    {
        SetHover(false);
    }

    public void SetHover(bool state)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsSceneAnswered(sceneName))
        {
            return;
        }
        if (isSelected || isLocked) return;

        isHovered = state;

        if (outline != null)
        { 
            ApplyPlayerColor();
            outline.enabled = state;
        }

        if (hoverText != null)
        {
            hoverText.SetVisible(state);
            hoverText.SetOutline(false);
        }
    }

    public void Select()
    {
        if (isLocked) return;
        if (GameManager.Instance != null && GameManager.Instance.IsSceneAnswered(sceneName))
        {
            return;
        }
        isSelected = true;
        isHovered = false;

        if (outline != null)
        {
            ApplyPlayerColor();
            outline.enabled = true;
        }

        if (hoverText != null)
        {
            hoverText.SetVisible(true);
            hoverText.SetOutline(true);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddSelection(sceneName, choiceLetter);
        }

        if (selectSound != null && audioSource != null)
            audioSource.PlayOneShot(selectSound);

        portal.SetActive(true);
    }
    public void Lock()
    {
        isLocked = true;
        isHovered = false;

        if (outline != null)
            outline.enabled = false;

        if (hoverText != null)
        {
            hoverText.SetVisible(false);
            hoverText.SetOutline(false);
        }
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
    void ApplyPlayerColor()
    {
        if (outline != null && GameManager.Instance != null)
        {
            outline.OutlineColor = GameManager.Instance.playerColor;
        }
    }
}
