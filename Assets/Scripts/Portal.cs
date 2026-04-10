using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad = "pastroom";

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("PORTAL HIT BY: " + other.name);
        //if (other.CompareTag("Player"))
        if (other.transform.root.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}