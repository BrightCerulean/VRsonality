using UnityEngine;

public class RayCast : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    public float maxDistance = 50f;
    public GameObject hitDot;
    public MenuController menuController;
    private Interactable currentHover;
    public static string AButton;

    //private OutlineHighlight currentOutline;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            if (GameManager.Instance != null)
            {
                Color c = GameManager.Instance.playerColor;
                lineRenderer.startColor = c;
                lineRenderer.endColor = c;
                
            }
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