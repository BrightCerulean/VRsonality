using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public AudioClip transitionSound;

    private bool transitioning = false;

    void Update()
    {
        if (transitioning) return;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.JoystickButton7))
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
        SceneManager.LoadScene(sceneToLoad);
    }
}
