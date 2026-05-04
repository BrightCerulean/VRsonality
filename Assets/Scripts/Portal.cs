using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    public string sceneToLoad = "pastroom";
    public AudioClip transitionSound;
    private bool transitioning = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("PORTAL HIT BY: " + other.name);
        if (transitioning) return;
        if (other.transform.root.CompareTag("Player"))
        {
            transitioning = true;
            StartCoroutine(LoadWithSound());
        }
    }

    IEnumerator LoadWithSound()
    {
        if (transitionSound != null)
        {
            AudioSource audio = GetComponent<AudioSource>();
            if (audio == null) audio = gameObject.AddComponent<AudioSource>();
            audio.PlayOneShot(transitionSound);
            yield return new WaitForSeconds(transitionSound.length);
        }
        GameManager.Instance.TransitionToScene(sceneToLoad);
    }
}