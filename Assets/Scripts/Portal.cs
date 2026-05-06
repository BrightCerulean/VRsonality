using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;

public class Portal : MonoBehaviour
{
    public string sceneToLoad = "pastroom";
    public AudioClip transitionSound;
    private bool transitioning = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("PORTAL HIT BY: " + other.name);
        if (transitioning) return;
        if (!other.transform.root.CompareTag("Player")) return;

        PlayerAvatar avatar = other.transform.root.GetComponent<PlayerAvatar>();
        if (avatar == null || !avatar.photonView.IsMine) return;

        transitioning = true;
        StartCoroutine(HandleTransition());
    }

    IEnumerator HandleTransition()
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
            Debug.LogWarning("[Portal] XRCardboardRig not found before scene load!");
        }

        PlayerAvatar myAvatar = FindFirstObjectByType<PlayerAvatar>();
        if (myAvatar != null && myAvatar.photonView.IsMine)
            PhotonNetwork.Destroy(myAvatar.gameObject);

        if (GameManager.Instance != null)
            GameManager.Instance.TransitionToScene(sceneToLoad);
        else
            SceneManager.LoadScene(sceneToLoad);
    }
}