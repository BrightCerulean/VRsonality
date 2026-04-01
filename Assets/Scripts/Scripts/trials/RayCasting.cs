using UnityEngine;
using UnityEngine.EventSystems;

public class RayCasting : MonoBehaviour
{
    
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    public float maxDistance = 50f;
    public OutlineHighlight outline, IPointerEnterHandler, IPointerExitHandler; 

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
        lineRenderer.positionCount = 2; 
        outline = GetComponent<OutlineHighlight>();
        outline.enabled = false;
    }
    
    void Update()
    {
        Debug.Log("RayCasting Update called");
        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, maxDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);
           /* if(hit.collider.gameObject.CompareTag("Interactable"))
                {
                    outline.enabled = true; 
                    Debug.Log("Outline enabled on: " + hit.collider.gameObject.name);
                }
                else
                {
                    outline.enabled = false; 
                    Debug.Log("Outline disabled on: " + hit.collider.gameObject.name);

                }*/
                //Debug.Log("Outline enabled on: " + hit.collider.gameObject.name);
        Debug.Log("Hit: " + hit.collider.gameObject.name);
        }
        else
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * maxDistance);
        }
    }  
     public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
    } 
}
/*using UnityEngine;

public class RayCasting : MonoBehaviour
{
    LayerMask layerMask;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Wall", "Character");
    }

    // See Order of Execution for Event Functions for information on FixedUpdate() and Update() related to physics queries
    void FixedUpdate()
    {

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))

        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }

    }
}
*/