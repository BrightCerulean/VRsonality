using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;
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
    public bool autoTransition = false;
    public string nextScene;
    private bool transitioning = false;

    void Start()
    {
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
            GameManager.Instance.PlaySelectSound();
        }

        if (autoTransition)
        {
            if (string.IsNullOrEmpty(nextScene))
            {
                Debug.LogWarning("[Interactable] nextScene is empty! Set it in the Inspector.");
                return;
            }
            transitioning = true;
            StartCoroutine(LoadNextScene());
        }
        else
        {
            if (portal != null)
                portal.SetActive(true);
            else
                Debug.LogWarning("[Interactable] autoTransition is false but no portal assigned.");
        }
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject rig = GameObject.Find("XRCardboardRig");
        if (rig != null)
        {
            rig.transform.SetParent(null);
            DontDestroyOnLoad(rig);
        }

        PlayerAvatar myAvatar = FindFirstObjectByType<PlayerAvatar>();
        if (myAvatar != null && myAvatar.photonView.IsMine)
            PhotonNetwork.Destroy(myAvatar.gameObject);

        Debug.Log("[Interactable] Loading scene: " + nextScene);

        if (GameManager.Instance != null)
            GameManager.Instance.TransitionToScene(nextScene);
        else
            SceneManager.LoadScene(nextScene);
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
