using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using Photon.Pun;

public class StartButton : MonoBehaviour
{
    public string sceneToLoad;
    public AudioClip transitionSound;
    public Color textColor = Color.white;
    public Color outlineColor = new Color(1f, 0.85f, 0.2f, 1f);
    [Range(0f, 1f)] public float outlineWidth = 0.2f;

    private TextMeshProUGUI label;

    void Start()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.color = textColor;
            label.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
        }
        else
            Debug.LogWarning("[StartButton] No TMP Text found on " + gameObject.name + " or children");
    }

    public void OnHoverEnter()
    {
        if (label != null)
        {
            label.color = outlineColor;
            label.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
            label.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineWidth);
        }
    }

    public void OnHoverExit()
    {
        if (label != null)
        {
            label.color = textColor;
            label.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
        }
    }

    public void OnSelect()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        if (transitionSound != null)
        {
            AudioSource audio = GetComponent<AudioSource>();
            if (audio == null) audio = gameObject.AddComponent<AudioSource>();
            audio.PlayOneShot(transitionSound);
            yield return new WaitForSeconds(transitionSound.length);
        }

        GameObject rig = GameObject.Find("XRCardboardRig");
        if (rig != null)
        {
            rig.transform.SetParent(null);
            DontDestroyOnLoad(rig);
        }
        else
        {
            Debug.LogWarning("[StartButton] XRCardboardRig not found before scene load!");
        }

        PlayerAvatar myAvatar = FindFirstObjectByType<PlayerAvatar>();
        if (myAvatar != null && myAvatar.photonView.IsMine)
            PhotonNetwork.Destroy(myAvatar.gameObject);

        SceneManager.LoadScene(sceneToLoad);
    }
}
