/*
using UnityEngine;

public class RayCast : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    public float maxDistance = 50f;
    public GameObject hitDot;

    private OutlineHighlight currentOutline; // track current object

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
        }
        else
        {
            Debug.LogError("LineRenderer missing!");
        }
    }

    void Update()
    {
        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, maxDistance))
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);

            Debug.Log("Hit: " + hit.collider.gameObject.name);

            OutlineHighlight newOutline = hit.collider.GetComponent<OutlineHighlight>();

            // check for object hit and enable outline
            if (currentOutline != newOutline)
            {
                if (currentOutline != null)
                {
                    currentOutline.enabled = false;
                }

                if (newOutline != null)
                {
                    newOutline.enabled = true;
                }

                currentOutline = newOutline;
            }
            if (hitDot != null) hitDot.SetActive(true);
            if (hitDot != null) hitDot.transform.position = hit.point;
        }
        else
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * maxDistance);

           
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
            if (hitDot != null) hitDot.SetActive(false);
        }
        Teleporting teleporting = hit.collider.GetComponent<Teleporting>();
        if (teleporting != null)
            {
                teleporting.TeleportTo(hit.point);
            }
    }
        
}

*/

using UnityEngine;

public class RayCast : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    public float maxDistance = 50f;
    public GameObject hitDot;
    public MenuController menuController;

    private OutlineHighlight currentOutline;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
        }
        else
        {
            Debug.LogError("LineRenderer missing!");
        }
    }

    void Update()
    {
        if (menuController != null) 
        {
            maxDistance = menuController.raycastDistance;
        }
        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;

        RaycastHit hit;

        int layerMask = ~LayerMask.GetMask("Character");
        //Debug.Log("LayerMask value: " + layerMask);
        if (Physics.Raycast(origin, direction, out hit, maxDistance, layerMask))
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);

            // Show hit dot
            if (hitDot != null) hitDot.SetActive(true);
            if (hitDot != null) hitDot.transform.position = hit.point;

            //Debug.Log("Hit: " + hit.collider.gameObject.name);

            // Outline highlight
            OutlineHighlight newOutline = hit.collider.GetComponent<OutlineHighlight>();
            if (currentOutline != newOutline)
            {
                if (currentOutline != null) currentOutline.enabled = false;
                if (newOutline != null) newOutline.enabled = true;
                currentOutline = newOutline;
            }

            // Teleport
            Teleporting teleporting = hit.collider.GetComponent<Teleporting>();
            if (teleporting != null)
            {
                // teleporting.TeleportTo(hit.point);
                bool isInteractable = hit.collider.GetComponent<ObjectMenu>() != null;
                teleporting.TeleportTo(hit.point, isInteractable);
            }
        }
        else
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * maxDistance);

            // Hide hit dot
            if (hitDot != null) hitDot.SetActive(false);

            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
        }
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown((KeyCode)(360 + i)))
            {
                Debug.Log("JoystickButton" + i + " pressed");
            }
        }
    }
}