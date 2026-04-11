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
    private HoverHighlight currentHighlight;

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

        if (lineRenderer != null && GameManager.Instance != null)
        {
            Color pc = GameManager.Instance.playerColor;
            lineRenderer.startColor = pc;
            lineRenderer.endColor = pc;
        }

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
            // Hover highlight
            HoverHighlight newHighlight = hit.collider.GetComponentInParent<HoverHighlight>();
            Debug.Log("[RayCast] HoverHighlight found: " + (newHighlight != null ? newHighlight.gameObject.name : "NULL"));

            if (currentHighlight != newHighlight)
            {
                Debug.Log("[RayCast] Switching highlight");
                if (currentHighlight != null) currentHighlight.OnHoverExit();
                if (newHighlight != null) newHighlight.OnHoverEnter();
                currentHighlight = newHighlight;
            }

            // Button press
            bool buttonPressed = Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.JoystickButton10) || Input.GetButtonDown(AButton);
            Debug.Log("[RayCast] ButtonPressed: " + buttonPressed);

            if (buttonPressed)
            {
                ColorSelector colorSelector = hit.collider.GetComponentInParent<ColorSelector>();
                Debug.Log("[RayCast] ColorSelector found: " + (colorSelector != null ? colorSelector.gameObject.name : "NULL"));
                if (colorSelector != null)
                {
                    colorSelector.OnSelect();
                    return;
                }

                SelectableObject selectable = hit.collider.GetComponentInParent<SelectableObject>();
                if (selectable != null)
                {
                    selectable.OnSelect();
                    return;
                }

                ImageUpload upload = hit.collider.GetComponent<ImageUpload>();
                if (upload != null)
                {
                    upload.OnSelect();
                    return;
                }
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
        /*
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
        */
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown((KeyCode)(360 + i)))
            {
                Debug.Log("JoystickButton" + i + " pressed");
            }
        }
        
    }
}