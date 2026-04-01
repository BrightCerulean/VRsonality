/*using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyAndSpawn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string yButton = "js5"; 
    private bool isPointing = false;
    public bool isFloor = false;
    public GameObject prefabReference;
    public static GameObject lastDestroyedPrefab;

    void Update()
    {
        if (Input.GetButtonDown(yButton)||Input.GetKey(KeyCode.K)) 
        {
            if (isPointing && !isFloor)
            {
                lastDestroyedPrefab = prefabReference;
                Destroy(gameObject);
            }
            else if (isPointing && isFloor && lastDestroyedPrefab != null)
            {
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Hit: " + hit.collider.gameObject.name + " Tag: " + hit.collider.tag);
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        Vector3 spawnPos = hit.point + Vector3.up * 0.5f;
                        Instantiate(lastDestroyedPrefab, spawnPos, lastDestroyedPrefab.transform.rotation);
                        lastDestroyedPrefab = null;
                        Debug.Log("Spawned!");
                    }
                }
                else if (isPointing && isFloor && lastDestroyedPrefab != null)
                {
                    Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
                    Instantiate(lastDestroyedPrefab, spawnPos, lastDestroyedPrefab.transform.rotation);
                    lastDestroyedPrefab = null;
                    Debug.Log("Spawned!");  
            }
                else
                {
                    Debug.Log("Raycast hit nothing!");
                }
            }
            Debug.Log("Trying to spawn: " + lastDestroyedPrefab);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) { isPointing = true; }
    public void OnPointerExit(PointerEventData eventData) { isPointing = false; }
}
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyAndSpawn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string yButton = "js5"; 
    private bool isPointing = false;
    public bool isFloor = false;

    public GameObject prefabReference;
    public static GameObject lastDestroyedPrefab;

    private static bool readyToSpawn = false;

    void Update()
    {
        if (Input.GetButtonDown(yButton) || Input.GetKeyDown(KeyCode.D)) 
        {
            // DESTROY (First Click)
            if (isPointing && !isFloor && !readyToSpawn)
            {
                lastDestroyedPrefab = prefabReference;
                readyToSpawn = true;
                Destroy(gameObject);
                Debug.Log("Destroyed - Ready to spawn");
            }

            // SPAWN (Second Click)
            else if (isPointing && isFloor && readyToSpawn && lastDestroyedPrefab != null)
            {
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Floor"))
                    {
                        Vector3 spawnPos = hit.point + Vector3.up * 0.5f;

                        Instantiate(
                            lastDestroyedPrefab,
                            spawnPos,
                            lastDestroyedPrefab.transform.rotation
                        );

                        readyToSpawn = false;
                        lastDestroyedPrefab = null;

                        Debug.Log("Spawned!");
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointing = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointing = false;
    }
}