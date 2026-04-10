using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectableObject : MonoBehaviour
{
    [Header("Selection")]
    [Tooltip("Set to H, C, E, or S")]
    public string selectionLetter = "H";

    [Tooltip("Describe what this object represents")]
    public string objectDescription = "";

    [Header("After Selection")]
    public GameObject portalToActivate;

    private bool hasBeenSelected = false;

    public void OnSelect()
    {
        if (hasBeenSelected) return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (GameManager.Instance.IsSceneAnswered(currentScene))
        {
            Debug.Log("[Selectable] Already answered");
            return;
        }

        GameManager.Instance.AddSelection(currentScene, selectionLetter);
        hasBeenSelected = true;

        Debug.Log("[SelectableObject] " + objectDescription +
            " → " + selectionLetter);

        if (portalToActivate != null)
            portalToActivate.SetActive(true);
    }
}