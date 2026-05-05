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
            if (GameManager.Instance != null)
                GameManager.Instance.TransitionToScene(sceneToLoad);
            else
                SceneManager.LoadScene(sceneToLoad);
        }
    }

}