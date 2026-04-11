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
    private Interactable currentHover;
    public string AButton;

    //private OutlineHighlight currentOutline;

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

    private void Awake()
    {
        if (Application.isEditor)//PC
        {
            AButton = "js3";
        }
        else//Android
        {
            AButton = "js0";
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
        bool didHit = Physics.Raycast(origin, direction, out hit, maxDistance, layerMask);
        //Debug.Log("LayerMask value: " + layerMask);
        if (didHit)
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);

            // Show hit dot
            if (hitDot != null)
            {
                hitDot.SetActive(true);
                hitDot.transform.position = hit.point;
            }
        }
        else
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * maxDistance);

            if (hitDot != null)
                hitDot.SetActive(false);
        }
        Interactable newHover = didHit ? hit.collider.GetComponentInParent<Interactable>() : null;
        //Hover
        if (newHover != currentHover)
        {
            if (currentHover != null)
                currentHover.SetHover(false);

            currentHover = newHover;

            if (currentHover != null)
                currentHover.SetHover(true);
        }
        //Select
        if (Input.GetButtonDown(AButton))
        {
            if (currentHover != null)
            {
                currentHover.Select();
                //Find all other interactable objects in the scene and lock them out
                Interactable[] all = FindObjectsByType<Interactable>(FindObjectsSortMode.None);

                foreach (var obj in all)
                {
                    if (obj != currentHover)
                        obj.Lock();
                }
            }
        }
        //Teleport
        if (didHit)
        {
            Teleporting teleporting = hit.collider.GetComponent<Teleporting>();
            if (teleporting != null)
            {
                bool isInteractable = hit.collider.GetComponent<ObjectMenu>() != null;
                teleporting.TeleportTo(hit.point, isInteractable);
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